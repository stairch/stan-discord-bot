<script setup lang="ts">
    import { computed, onMounted, ref, watch } from "vue";
    import { useRouter } from "vue-router";
    import { useAnnouncementStore } from "@/stores/announcements";
    import { api, type IAnnouncementSummary } from "@/api";
    import { debounce, throttle } from "@/assets/debounce";
    import { isTemplateSpan } from "typescript";

    const announcementStore = useAnnouncementStore();
    const router = useRouter();

    const PAGE_SIZE = 8;

    const announcementCount = ref(0);
    const page = ref(0);
    const announcements = ref<IAnnouncementSummary[]>([]);

    const sortedAnnouncements = computed(() => {
        return announcements.value.sort((a, b) => {
            return (
                new Date(b.lastModified).getTime() -
                new Date(a.lastModified).getTime()
            );
        });
    });

    const firstAnnouncement = computed(() => sortedAnnouncements.value?.[0]);

    const createAnnouncement = async () => {
        const href = await announcementStore.create();
        router.push(href);
    };

    const filter = ref("");
    const onlyMe = ref(false);

    const searchAnnouncements = async () => {
        const { items, totalCount } = await api.announements.search(
            filter.value,
            onlyMe.value,
            undefined,
            PAGE_SIZE,
            offset.value
        );
        announcements.value = items;
        announcementCount.value = totalCount;
    };

    const searchAnnouncementsThrottled = throttle(searchAnnouncements, 500);
    const searchAnnouncementsDebounced = debounce(searchAnnouncements, 500);

    watch(filter, searchAnnouncementsDebounced);
    watch(onlyMe, searchAnnouncementsThrottled);

    const pageCount = computed(() => {
        return Math.ceil(announcementCount.value / PAGE_SIZE);
    });

    const offset = computed(() => {
        return page.value * PAGE_SIZE;
    });

    const switchToPage = (newPage: number) => {
        if (newPage < 0 || newPage >= pageCount.value) {
            return;
        }

        page.value = newPage;
        searchAnnouncementsThrottled();
    };

    onMounted(() => {
        searchAnnouncementsThrottled();
    });
</script>
<template>
    <div class="root">
        <h1>Announcements</h1>
        <div class="announcements">
            <div class="cards">
                <div
                    class="card"
                    @click="createAnnouncement"
                >
                    <div class="icon">
                        <span class="material-symbols-rounded"> add </span>
                    </div>
                    <h3>New Announcement</h3>
                    <p>Create a new announcement from scratch!</p>
                </div>
                <div
                    class="card"
                    disabled="true"
                >
                    <div class="icon">
                        <span class="material-symbols-rounded">
                            construction
                        </span>
                    </div>
                    <h3>Not yet implemented</h3>
                    <p>
                        post one-time announcements on Discord, edit sent
                        announcements & more
                    </p>
                </div>
            </div>

            <div class="filters">
                <input
                    type="text"
                    v-model="filter"
                    placeholder="Search..."
                />
                <label
                    @click="onlyMe = !onlyMe"
                    :aria-checked="onlyMe"
                    :title="
                        onlyMe
                            ? 'Show all'
                            : 'Show only announcements I last modified'
                    "
                >
                    <span class="material-symbols-rounded"> person </span>
                    My announcements
                </label>
            </div>

            <div
                class="itemlist"
                v-if="announcements.length"
            >
                <div class="announcement">
                    <a>
                        <span>Name</span>
                        <span>Date modified</span>
                        <span>Modified by</span>
                    </a>
                </div>
                <div
                    v-for="availableAnnouncement in sortedAnnouncements"
                    :key="availableAnnouncement.id"
                    class="announcement"
                >
                    <router-link
                        :to="`/announcements/${availableAnnouncement.id}`"
                    >
                        <h3 v-if="availableAnnouncement.title">
                            {{ availableAnnouncement.title }}
                        </h3>
                        <h3 v-else><i>N/A</i></h3>
                        <span>{{
                            new Date(
                                availableAnnouncement.lastModified
                            ).toLocaleString()
                        }}</span>
                        <span>{{ availableAnnouncement.author }}</span>
                    </router-link>
                </div>
            </div>
            <div
                v-if="announcements.length"
                class="pagination"
            >
                <span>
                    Showing <strong>{{ announcements.length }}</strong> of
                    <strong>{{ announcementCount }}</strong> announcements
                </span>
                <div class="pager">
                    <span
                        class="material-symbols-rounded"
                        :disabled="page === 0"
                        @click="switchToPage(page - 1)"
                    >
                        chevron_left
                    </span>
                    <span>Page {{ page + 1 }} of {{ pageCount }}</span>
                    <span
                        class="material-symbols-rounded"
                        :disabled="page === pageCount - 1"
                        @click="switchToPage(page + 1)"
                    >
                        chevron_right
                    </span>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
    .root {
        height: calc(100% - 2rem);
        overflow: clip;
        display: flex;
        flex-direction: column;
    }

    .itemlist {
        height: calc(100% - 2rem);
        max-height: calc(100% - 2rem);
        overflow-y: scroll;
        flex: 1;
    }

    .card {
        min-width: 30ch;
        flex: 1;
        display: flex;
        flex-direction: column;
        justify-content: flex-end;
        gap: 0.5em;
        cursor: pointer;
        color: var(--fg-text-muted);

        &:not([disabled="true"]):hover {
            background-color: var(--bg-muted);
        }

        &[disabled="true"] {
            cursor: not-allowed;
            color: var(--fg-text-muted);
        }

        .icon {
            flex: 1;
            align-self: center;
        }

        .material-symbols-rounded {
            font-size: 5rem;
        }
    }

    .pagination {
        display: flex;
        justify-content: space-between;
        gap: 1em;
        color: var(--fg-text-muted);

        & .pager {
            display: flex;
            gap: 1em;
            align-items: center;

            & span.material-symbols-rounded {
                cursor: pointer;
                font-size: 1rem;
                color: var(--fg-text);

                &:hover {
                    color: var(--c-stair-green);
                }

                &[disabled="true"] {
                    cursor: not-allowed;
                    color: var(--fg-text-muted);
                }
            }
        }
    }

    .filters {
        display: flex;
        gap: 1em;
        margin-top: 1em;

        & input {
            flex: 1;
            padding: 0.5em;
            border-radius: 4px;
            border: 1px solid var(--bg-muted);
        }

        & label {
            display: flex;
            gap: 0.5em;
            padding: 0.5em;
            border-radius: 4px;
            cursor: pointer;
            align-items: center;

            & span {
                font-size: 1rem;
            }

            &[aria-checked="true"] {
                color: var(--c-stair-green);
                background-color: var(--bg-muted);
            }

            &:hover {
                background-color: var(--c-stair-green-20);
            }
        }
    }

    .announcement {
        padding: 1em;
        border-radius: 4px;

        & a {
            display: grid;
            grid-template-columns: 1fr 20ch 10ch;
            color: var(--fg-text-muted);

            @media (max-width: 800px) {
                grid-template-columns: 1fr;

                & h3 {
                    font-size: 1rem;
                }

                & span:not(:first-child) {
                    display: none;
                }
            }
        }

        &:hover {
            background-color: var(--bg-muted);
        }

        &:not(:last-child) {
            border-bottom: 1px solid var(--bg-muted);
        }
    }

    .announcements {
        display: flex;
        flex-direction: column;
        gap: 2em;
        min-height: calc(100% - 2rem);
    }

    .cards {
        display: flex;
        flex-direction: row;
        gap: 1em;

        @media (max-width: 800px) {
            flex-direction: column;
        }
    }
</style>
