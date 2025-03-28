<script setup lang="ts">
import { watch, ref, type PropType, onMounted } from "vue";
import { toUnicodeVariant } from "@/assets/toUnicodeVariant";
import Step from "@/components/Step.vue";
import ToastNotification from "@/components/ToastNotification.vue";
import type { IAnnouncement } from "@/api";
import { actualMarkdownToMarkdownV2 } from "@/assets/markdown";

const text = ref<string>("");
const toast = ref<typeof ToastNotification>();

const props = defineProps({
    modelValue: { type: Object as PropType<IAnnouncement>, required: true },
});

const buildText = () => {
    const fullText =
        props.modelValue.message.de +
        "\n\n---\n\n" +
        props.modelValue.message.en;

    console.log(fullText, actualMarkdownToMarkdownV2(fullText));

    return actualMarkdownToMarkdownV2(fullText);
};

const copyToClipbaord = () => {
    navigator.clipboard.writeText(text.value);
    toast.value?.showToast({
        message: "Copied to Clipboard!",
        style: "success",
        icon: "done",
    });
};

onMounted(async () => {
    text.value = buildText();
});

watch(
    () => props.modelValue,
    () => {
        text.value = buildText();
    },
    { deep: true }
);
</script>

<template>
    <ToastNotification ref="toast" />
    <div class="checklist">
        <h2>Checklist</h2>
        <div class="checklist">
            <Step title="Add 16:9 Image" description="Because people are lazy" />
            <Step title="Past Caption" description="Full event description" />
        </div>
    </div>
    <h2>Message</h2>
    <div class="textarea">
        <textarea class="instagram" v-model="text"></textarea>
        <span class="material-symbols-rounded" @click="copyToClipbaord" title="Copy All">
            content_copy
        </span>
    </div>
</template>

<style scoped>
textarea.instagram {
    height: 50ch;
    width: 100%;
}

.checklist {
    display: flex;
    flex-direction: column;
    gap: 1em;
}

.textarea {
    position: relative;
    width: 100%;

    .material-symbols-rounded {
        position: absolute;
        right: 0.5em;
        top: 0.5em;
        cursor: pointer;
        background: var(--bg-base);
        padding: 0.25em;
        font-size: 1.8em;
        border: 1px solid var(--bg-muted);
        border-radius: 0.25em;

        &:hover {
            color: var(--c-accent);
            border-color: var(--c-accent);
        }
    }
}
</style>
