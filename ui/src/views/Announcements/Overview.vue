<script setup lang="ts">
import { computed } from "vue";
import { useRouter } from "vue-router";
import { useAnnouncementStore } from "@/stores/announcements";

const announcementStore = useAnnouncementStore();
const router = useRouter();

const sortedAnnouncements = computed(() => {
    return announcementStore.announcements.sort((a, b) => {
        return new Date(b.lastModified).getTime() - new Date(a.lastModified).getTime();
    });
});

const firstAnnouncement = computed(() => sortedAnnouncements.value?.[0]);

const createAnnouncement = async () => {
    const href = await announcementStore.create();
    router.push(href);
};
</script>
<template>
    <h1>Announcements</h1>
    <div class=announcements>
        <div class="cards">
            <div class=card @click="createAnnouncement">
                <div class="icon">
                    <span class="material-symbols-rounded">
                        add
                    </span>
                </div>
                <h3>New Announcement</h3>
            </div>
            <router-link
                v-if="firstAnnouncement"
                :to="`/announcements/${firstAnnouncement.id}`"
                class=card
            >
                <h3>{{ firstAnnouncement.title }}</h3>
                <p class="text-muted">
                    last modified on
                    <strong>{{ new Date(firstAnnouncement.lastModified).toLocaleString() }}</strong>
                    by <strong>{{ firstAnnouncement.author }}</strong>
                </p>
            </router-link>
        </div>

        <div class="itemlist" v-if="announcementStore.announcements.length > 1">
            <div
                class="announcement"
            >
                <a>
                    <span>Name</span>
                    <span>Date modified</span>
                    <span>Modified by</span>
                </a>
            </div>
            <div
                v-for="availableAnnouncement in sortedAnnouncements.slice(1)"
                :key="availableAnnouncement.id"
                class="announcement"
            >
                <router-link
                    :to="`/announcements/${availableAnnouncement.id}`"
                >
                    <h3 v-if="availableAnnouncement.title">{{ availableAnnouncement.title }}</h3>
                    <h3 v-else><i>N/A</i></h3>
                    <span>{{ new Date(availableAnnouncement.lastModified).toLocaleString() }}</span>
                    <span>{{ availableAnnouncement.author }}</span>
                </router-link>
            </div>
        </div>
    </div>
</template>

<style scoped>
.card {
    min-width: 30ch;
    flex: 1;
    display: flex;
    flex-direction: column;
    justify-content: flex-end;
    gap: 0.5em;
    cursor: pointer;
    color: var(--fg-text-muted);

    &:hover {
        background-color: var(--bg-muted);
    }

    .icon {
        flex: 1;
        align-self: center;
    }

    .material-symbols-rounded {
        font-size: 5rem;
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
