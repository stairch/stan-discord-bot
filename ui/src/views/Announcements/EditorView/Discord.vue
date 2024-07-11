<script setup lang="ts">
    import { computed, watch, ref, type PropType, onMounted } from "vue";
    import {
        api,
        ANNOUNCEMENT_TYPES,
        type IServer,
        type IAnnouncement,
    } from "@/api";
    import {
        DiscordMarkdown,
        DiscordEmbed,
        DiscordMention,
        DiscordEmbedField,
        DiscordMessage,
        DiscordMessages,
        // @ts-ignore
    } from "@discord-message-components/vue";
    import LoadingWithResultModal from "@/components/LoadingWithResultModal.vue";

    const modal = ref<InstanceType<typeof LoadingWithResultModal> | null>(null);
    const servers = ref<IServer[]>([]);
    const personas = ref<string[]>([]);
    const server = ref<string>("");
    const persona = ref<string>("");
    const type = ref<string>(ANNOUNCEMENT_TYPES[0]);

    const props = defineProps({
        modelValue: { type: Object as PropType<IAnnouncement>, required: true },
    });
    const announcement = computed(() => ({ ...props.modelValue }));

    const img = ref<File | null>(null);

    const imgUrl = ref<string | null>(null);

    onMounted(async () => {
        servers.value = await api.announements.discordServers();
        server.value = servers.value[0].id;
        personas.value = await api.announements.personas();
        persona.value = personas.value[0];
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
        modal.value!.onLoading();

        const error = await api.announements.publish(
            announcement.value.id!,
            "discord",
            String(servers.value[0].id),
            type.value,
            persona.value,
            img.value ?? undefined
        );
        if (!error) {
            modal.value!.onSuccess("Announcement posted!");
        } else {
            modal.value!.onError(error);
        }
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
    <LoadingWithResultModal ref="modal" />
    <div class="inputs">
        <div class="dropdown">
            <label>On Server</label>
            <select v-model="server">
                <option
                    v-for="s in servers"
                    :key="s.id"
                    :value="s.id"
                >
                    {{ s.name }}
                </option>
            </select>
        </div>
        <div class="dropdown">
            <label>As</label>
            <select v-model="type">
                <option
                    v-for="t in ANNOUNCEMENT_TYPES"
                    :key="t"
                    :value="t"
                >
                    {{ t }}
                </option>
            </select>
        </div>
        <div class="dropdown">
            <label>Post as</label>
            <select v-model="persona">
                <option
                    v-for="t in personas"
                    :key="t"
                    :value="t"
                >
                    {{ t }}
                </option>
            </select>
        </div>
        <button
            @click="setImage"
            class="align-right secondary"
        >
            Set Image
        </button>
    </div>
    <div>
        <DiscordMessages>
            <DiscordMessage
                :bot="true"
                :author="persona"
                role-color="#04956c"
                :key="persona"
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

    .align-right {
        margin-left: auto;
    }

    .dropdown {
        display: flex;
        flex-direction: column;
        gap: 0.5em;
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
