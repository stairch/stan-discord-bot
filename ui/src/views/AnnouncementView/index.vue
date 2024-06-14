<script setup lang="ts">
import { onMounted, ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import { api, type IAnnouncement } from "../../api";
import Edit from "./Edit.vue";
import Discord from "./Discord.vue";
import { useAnnouncementStore } from "@/stores/announcements";
import Instagram from "./Instagram.vue";

const announcement = ref<IAnnouncement>({
    title: "",
    message: {
        en: "",
        de: "",
    }
});

const route = useRoute();
const router = useRouter();
const announcementStore = useAnnouncementStore();

const TABS = {
    Edit: Edit,
    "Post on Discord": Discord,
    "Post on Instagram": Instagram,
};
let requestTab = route.query.tab as string;
const activeTab = ref<keyof typeof TABS>("Edit");
if (Object.keys(TABS).includes(requestTab)) {
    activeTab.value = requestTab as keyof typeof TABS;
}

watch(activeTab, (x) => {
    const query = { tab: x };
    router.replace({ query });
});

onMounted(async () => {
    const announcementId = Number(route.params.id);
    if (announcementId) {
        announcement.value = await api.announements.get(announcementId);
    }
});

watch(route, async () => {
    const announcementId = Number(route.params.id);
    if (announcementId) {
        announcement.value = await api.announements.get(announcementId);
    } else {
        announcement.value = {
            title: "",
            message: {
                en: "",
                de: "",
            }
        };
    }
});
</script>

<template>
    <h1>Announcements</h1>
    <div class="announcement">
        <aside>
            <router-link
                v-for="availableAnnouncement in announcementStore.announcements"
                :key="availableAnnouncement.id"
                :to="`/announcements/${availableAnnouncement.id}`"
            >
                <div
                    class="server"
                    :class="{
                        selected: announcement?.id == availableAnnouncement.id,
                    }"
                >
                    <span>{{ availableAnnouncement.title }}</span>
                </div>
            </router-link>
            <router-link to="/announcements">
                <div
                    class="server"
                    :class="{
                        selected: announcement.id == null,
                    }"
                >
                    <span>+ New Announcement</span>
                </div>
            </router-link>
        </aside>
        <main>
            <div class="tab-list">
                <span
                    v-for="key in Object.keys(TABS)"
                    @click="activeTab = key as keyof typeof TABS"
                    :class="{ active: key === activeTab }"
                >
                    {{ key }}
                </span>
            </div>
            <component
                :is="TABS[activeTab]"
                v-model="announcement"
            />
        </main>
    </div>
</template>

<style>
.announcement {
    display: grid;
    grid-template-columns: 25ch 1fr;
    align-items: start;
    gap: 1em;
}

main {
    display: flex;
    flex-direction: column;
    gap: 1em;
}

aside {
    background-color: var(--bg-soft);
    border: 1px solid var(--bg-muted);
    border-radius: 0.5em;

    & a {
        color: inherit;
    }
}

.server {
    display: flex;
    align-items: center;
    padding: 1em;
    gap: 1em;
    border-radius: 0.5em;

    &.selected {
        background-color: var(--bg-muted);
        color: var(--c-accent);
    }

    & img {
        width: 2em;
        height: 2em;
        border-radius: 0.25em;
    }
}

.actions {
    display: flex;
    gap: 1em;
    justify-content: flex-end;
}

.tab-list {
    display: flex;

    & span {
        padding: 1em 2em;
        cursor: pointer;
        border-bottom: 1px solid var(--bg-muted);
        transition: all 0.2s;
        border-radius: 0.5em 0.5em 0 0;

        &.active {
            border-bottom: 1px solid var(--c-accent);
        }

        &:hover {
            background-color: var(--bg-muted);
            color: var(--c-accent);
        }
    }
}
</style>
