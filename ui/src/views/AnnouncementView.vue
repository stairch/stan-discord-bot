<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useRoute, useRouter } from "vue-router";

interface IServer {
    name: string;
    picture: string;
}

const servers = ref<IServer[]>([]);
const channels = ref<string[]>([]);

const title = ref<string>("");
const contentEn = ref<string>("");
const contentDe = ref<string>("");
const server = ref<string>("");
const imgBase64 = ref<string>("");
const route = useRoute();
const router = useRouter();

onMounted(async () => {
    let res = await fetch("/api/announcement/servers");
    servers.value = await res.json();
    res = await fetch("/api/announcement/channels");
    channels.value = await res.json();

    const selectedServer = route.params.server as string;
    if (!selectedServer) {
        console.log(servers.value[0].name);
        router.push(`/announcement/${servers.value[0].name}`);
    }
    server.value = selectedServer || servers.value[0].name;
});

const postAnnouncement = async (type: string) => {
    const res = await fetch("/api/announcement", {
        method: "POST",
        body: JSON.stringify({
            title: title.value,
            message: {
                en: contentEn.value,
                de: contentDe.value,
            },
            server: server.value,
            channel: type,
            image: imgBase64.value.replace(/^data:image\/[a-z]+;base64,/, ""),
        }),
    });
    if (res.ok) {
        title.value = "";
        contentEn.value = "";
        contentDe.value = "";
    }
};

const actionsDisabled = computed(() => {
    return !title.value || !contentEn.value || !contentDe.value || !server;
});

function getBaseUrl(e: Event) {
    var reader = new FileReader();
    reader.onloadend = function () {
        imgBase64.value = String(reader.result);
        console.log(imgBase64.value);
    };
    reader.readAsDataURL((e.target as HTMLInputElement).files![0]);
}
</script>

<template>
    <h1>Announcements</h1>
    <div class="announcement">
        <aside>
            <router-link
                v-for="serverOption in servers"
                :key="serverOption.name"
                :to="`/announcement/${serverOption.name}`"
            >
                <div
                    class="server"
                    :class="{ selected: server == serverOption.name }"
                >
                    <img
                        :src="serverOption.picture"
                        alt="server"
                    />
                    <span>{{ serverOption.name }}</span>
                </div>
            </router-link>
        </aside>
        <main>
            <input
                type="text"
                v-model="title"
                placeholder="Title"
            />
            <textarea
                v-model="contentDe"
                placeholder="German Content"
            ></textarea>
            <textarea
                v-model="contentEn"
                placeholder="English Content"
            >
            </textarea>
            <input
                type="file"
                accept="image/*"
                @change="getBaseUrl"
            />
            <div class="actions">
                <button
                    @click="postAnnouncement('stair-announcements')"
                    class="danger"
                    :disabled="actionsDisabled"
                >
                    Post as STAIR Announcement
                </button>
                <button
                    @click="postAnnouncement('non-stair-announcements')"
                    class="danger"
                    :disabled="actionsDisabled"
                >
                    Post as Non-STAIR Announcement
                </button>
                <button
                    @click="postAnnouncement('server-info')"
                    class="danger"
                    :disabled="actionsDisabled"
                >
                    Post as Server Announcement
                </button>
                <button
                    @click="postAnnouncement('webhook-test')"
                    class="good"
                    :disabled="actionsDisabled"
                >
                    Test Announcement
                </button>
            </div>
        </main>
    </div>
</template>

<style>
.announcement {
    display: grid;
    grid-template-columns: max-content 1fr;
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
    padding: 0.5em;
    gap: 1em;
    border-radius: 0.5em;

    &.selected {
        background-color: var(--c-stair-grey);
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
