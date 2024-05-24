<script setup lang="ts">
import { computed, onMounted, ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import {
    api,
    type IServer,
    type IAnnouncementSummary,
    type IAnnouncement,
} from "../api";
import CardPicker from "@/components/CardPicker.vue";

const servers = ref<IServer[]>([]);
const types = ref<string[]>([]);
const announcements = ref<IAnnouncementSummary[]>([]);
const announcement = ref<IAnnouncement>({
    title: "",
    message: {
        en: "",
        de: "",
    },
    announcement_type: "test",
});

const img = ref<File | null>(null);
const route = useRoute();
const router = useRouter();

onMounted(async () => {
    servers.value = await api.announements.discordServers();
    types.value = await api.announements.getTypes();
    announcements.value = await api.announements.getAll();

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
            },
            announcement_type: "test",
        };
    }
});

const postAnnouncement = async () => {
    if (announcement.value.id) {
        await api.announements.update(announcement.value);
    } else {
        announcement.value = await api.announements.create(announcement.value);
    }

    await api.announements.publish(
        announcement.value.id!,
        "discord",
        String(servers.value[0].id),
        img.value ?? undefined
    );
};

const save = async () => {
    if (announcement.value.id) {
        await api.announements.update(announcement.value);
    } else {
        announcement.value = await api.announements.create(announcement.value);
    }
};

const saveDisabled = computed(() => {
    return (
        !announcement.value.title ||
        !(announcement.value.message?.en || announcement.value.message?.de)
    );
});

const actionsDisabled = computed(() => {
    return false;
});
</script>

<template>
    <h1>Announcements</h1>
    <div class="announcement">
        <aside>
            <router-link
                v-for="availableAnnouncement in announcements"
                :key="availableAnnouncement.id"
                :to="`/announcement/${availableAnnouncement.id}`"
            >
                <div
                    class="server"
                    :class="{
                        selected: announcement.id == availableAnnouncement.id,
                    }"
                >
                    <span>{{ availableAnnouncement.title }}</span>
                </div>
            </router-link>
            <router-link to="/announcement">
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
            <CardPicker
                v-model="announcement.announcement_type"
                :options="types.map((type) => ({ value: type, label: type }))"
            />
            <input
                type="text"
                v-model="announcement.title"
                placeholder="Title"
            />
            <textarea
                v-model="announcement.message.de"
                placeholder="German Content"
            ></textarea>
            <textarea
                v-model="announcement.message.en"
                placeholder="English Content"
            >
            </textarea>
            <input
                type="file"
                accept="image/*"
                @change="
                    img = ($event.target as HTMLInputElement).files?.[0] ?? null
                "
            />
            <div class="actions">
                <button
                    @click="save"
                    class="danger"
                    :disabled="saveDisabled"
                >
                    {{ announcement.id ? "Update" : "Create" }}
                </button>
                <button
                    @click="api.announements.delete(announcement.id!)"
                    class="danger"
                    :disabled="!announcement.id"
                >
                    Delete
                </button>
                <button
                    @click="postAnnouncement"
                    class="danger"
                    :disabled="actionsDisabled"
                >
                    Publish
                </button>
            </div>
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
}

.server {
    display: flex;
    align-items: center;
    padding: 1em;
    gap: 1em;
    border-radius: 0.5em;

    &.selected {
        background-color: var(--bg-muted);
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
</style>
