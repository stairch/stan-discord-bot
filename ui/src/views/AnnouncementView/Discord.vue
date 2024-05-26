<script setup lang="ts">
import { computed, watch, ref, type PropType, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import { api, type IServer, type IAnnouncement } from "../../api";
import {
    DiscordMarkdown,
    DiscordEmbed,
    DiscordMention,
    DiscordEmbedField,
    DiscordMessage,
    DiscordMessages,
} from "@discord-message-components/vue";
import CardPicker from "@/components/CardPicker.vue";

const servers = ref<IServer[]>([]);
const server = ref<string>("");

const props = defineProps({
    modelValue: { type: Object as PropType<IAnnouncement>, required: true },
});
const announcement = computed(() => ({ ...props.modelValue }));

const img = ref<File | null>(null);

const imgUrl = ref<string | null>(null);

onMounted(async () => {
    servers.value = await api.announements.discordServers();
    server.value = servers.value[0].id;
});

watch(
    () => img.value,
    async (value) => {
        if (value) {
            imgUrl.value = URL.createObjectURL(value);
        } else {
            imgUrl.value = null;
        }
    }
);

const postAnnouncement = async () => {
    await api.announements.publish(
        announcement.value.id!,
        "discord",
        String(servers.value[0].id),
        img.value ?? undefined
    );
};

const actionsDisabled = computed(() => {
    return false;
});

const setImage = () => {
    const input = document.createElement("input");
    input.type = "file";
    input.accept = "image/*";
    input.onchange = (e) => {
        const files = (e.target as HTMLInputElement).files;
        if (files && files.length > 0) {
            img.value = files[0];
        }
    };
    input.click();
};
</script>

<template>
    <div class="inputs">
        <CardPicker
            v-model="server"
            :options="servers.map((s) => ({ value: s.id, label: s.name }))"
            label="Server"
            key="id"
            value="name"
        />
        <button
            @click="setImage"
            class="warning"
        >
            Set Image
        </button>
    </div>
    <div>
        <DiscordMessages>
            <DiscordMessage
                :bot="true"
                author="Stan"
                role-color="green"
            >
                <DiscordMarkdown>
                    <DiscordMention type="Announcements" />
                </DiscordMarkdown>
                <img
                    v-if="imgUrl"
                    :src="imgUrl"
                    draggable="false"
                />
                <DiscordEmbed>
                    <DiscordEmbedField color="#0b6a5c">
                        <DiscordMarkdown>
                            <img
                                src="https://cdnjs.cloudflare.com/ajax/libs/twemoji/14.0.2/svg/1f1e9-1f1ea.svg"
                                alt="🇩🇪"
                                title="flag_de"
                                draggable="false"
                                class="flag"
                            />
                            **{{ announcement.title }}**
                            <br />
                            {{ announcement.message.de }}
                        </DiscordMarkdown>
                    </DiscordEmbedField>
                </DiscordEmbed>
                <DiscordEmbed>
                    <DiscordEmbedField color="#0b6a5c">
                        <DiscordMarkdown
                            ><img
                                src="https://cdnjs.cloudflare.com/ajax/libs/twemoji/14.0.2/svg/1f1ec-1f1e7.svg"
                                alt="🇬🇧"
                                title="flag_gb"
                                draggable="false"
                                class="flag"
                            />
                            **{{ announcement.title }}**
                            <br />
                            {{ announcement.message.en }}
                        </DiscordMarkdown>
                    </DiscordEmbedField>
                </DiscordEmbed>
            </DiscordMessage>
        </DiscordMessages>
    </div>

    <div class="actions">
        <button
            @click="postAnnouncement"
            class="danger"
            :disabled="actionsDisabled"
        >
            Publish
        </button>
    </div>
</template>

<style scoped>
.flag {
    width: 2ch;
    transform: translateY(0.25em);
}

img:not(.flag) {
    max-width: 75ch;
    border-radius: 0.5em;
    display: block;
    z-index: -1;
}

.inputs {
    display: flex;
    gap: 1em;
    align-items: center;
}
</style>

<style>
.discord-messages {
    border-radius: 0.5em;
    box-shadow: 0 0 20px 10px rgba(0, 0, 0, 0.15);
}

.discord-embed .discord-embed-left-border {
    background-color: #0b6a5c;
}
</style>
