<script setup lang="ts">
    import { computed, watch, ref, type PropType, onMounted } from "vue";
    import { api, type IServer, type IAnnouncement } from "@/api";
    import VueMarkdown from "vue-markdown-render";
    import LoadingWithResultModal from "@/components/LoadingWithResultModal.vue";

    const modal = ref<InstanceType<typeof LoadingWithResultModal> | null>(null);
    const servers = ref<IServer[]>([]);
    const server = ref<string>("");

    const props = defineProps({
        modelValue: { type: Object as PropType<IAnnouncement>, required: true },
    });
    const announcement = computed(() => ({ ...props.modelValue }));

    const img = ref<File | null>(null);

    const imgUrl = ref<string | null>(null);

    onMounted(async () => {
        servers.value = await api.announements.telegramChats();
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
        modal.value!.onLoading();

        const error = await api.announements.publish(
            announcement.value.id!,
            "telegram",
            server.value,
            "",
            "",
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

    const fullMessage = computed(() => {
        return (
            `**${announcement.value.title}**\n\n` +
            announcement.value.message.de +
            "\n\n--\-\n\n" +
            announcement.value.message.en
        ).replaceAll("\n", "<br/>");
    });

    const options = { html: true, breaks: true };
</script>

<template>
    <LoadingWithResultModal ref="modal" />
    <div class="inputs">
        <div class="dropdown">
            <label>Channel</label>
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
        <button
            @click="setImage"
            class="align-right secondary"
        >
            Set Image
        </button>
    </div>
    <div class="message">
        <img
            v-if="imgUrl"
            :src="imgUrl"
        />
        <div class="text">
            <VueMarkdown
                :source="fullMessage"
                :options="options"
            />
        </div>
        <svg
            width="9"
            height="20"
            class="svg-appendix"
        >
            <defs>
                <filter
                    x="-50%"
                    y="-14.7%"
                    width="200%"
                    height="141.2%"
                    filterUnits="objectBoundingBox"
                    id="messageAppendix"
                >
                    <feOffset
                        dy="1"
                        in="SourceAlpha"
                        result="shadowOffsetOuter1"
                    ></feOffset>
                    <feGaussianBlur
                        stdDeviation="1"
                        in="shadowOffsetOuter1"
                        result="shadowBlurOuter1"
                    ></feGaussianBlur>
                    <feColorMatrix
                        values="0 0 0 0 0.0621962482 0 0 0 0 0.138574144 0 0 0 0 0.185037364 0 0 0 0.15 0"
                        in="shadowBlurOuter1"
                    ></feColorMatrix>
                </filter>
            </defs>
            <g
                fill="none"
                fill-rule="evenodd"
            >
                <path
                    d="M3 17h6V0c-.193 2.84-.876 5.767-2.05 8.782-.904 2.325-2.446 4.485-4.625 6.48A1 1 0 003 17z"
                    fill="#000"
                    filter="url(#messageAppendix)"
                ></path>
                <path
                    d="M3 17h6V0c-.193 2.84-.876 5.767-2.05 8.782-.904 2.325-2.446 4.485-4.625 6.48A1 1 0 003 17z"
                    fill="FFF"
                    class="corner"
                ></path>
            </g>
        </svg>
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

    .message {
        display: flex;
        flex-direction: column;
        gap: 1em;
        align-items: center;
        background: var(--bg-soft);
        border-radius: 0.5em;
        border: 1px solid var(--bg-muted);
        max-width: 50ch;
        position: relative;
        border-bottom-left-radius: 0;

        .text {
            padding: 1em;
        }

        .svg-appendix {
            position: absolute;
            left: -9px;
            bottom: -4px;

            .corner {
                fill: var(--bg-muted);
            }
        }

        img {
            max-width: 100%;
            z-index: 1;
            border-radius: 0.5em 0.5em 0 0;
        }
    }
</style>
