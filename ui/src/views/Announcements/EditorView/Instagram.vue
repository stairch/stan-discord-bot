<script setup lang="ts">
import { watch, ref, type PropType, onMounted } from "vue";
import { toUnicodeVariant } from "@/assets/toUnicodeVariant";
import Step from "@/components/Step.vue";
import type { IAnnouncement } from "@/api";

const text = ref<string>("");

const props = defineProps({
    modelValue: { type: Object as PropType<IAnnouncement>, required: true },
});

const buildText = () => {
    const fullText =
        props.modelValue.message.de +
        "\n\n---\n\n" +
        props.modelValue.message.en;

    // replace markdown with unicode

    // bold text is surrounded by * or **
    const boldRegex = /\*\*(.*?)\*\*|\*(.*?)\*/g;
    const boldReplacer = (match: string, p1: string) => {
        return toUnicodeVariant(p1, "bold sans");
    };
    const boldText = fullText.replace(boldRegex, boldReplacer);

    // italic text is surrounded by _ or __
    const italicRegex = /__(.*?)__|_(.*?)_/g;
    const italicReplacer = (match: string, p1: string) => {
        return toUnicodeVariant(p1, "italic sans");
    };
    const italicText = boldText.replace(italicRegex, italicReplacer);

    return italicText;
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
    <div class="checklist">
        <h2>Checklist</h2>
        <div class=checklist>
            <Step
                title="Add Location"
                description="Where is the Event going to be?"
            />
            <Step
                title="Add Reminder"
                description="People will be able to set a reminder for the Event."
            />
            <Step
                title="Tag Accounts (optional)"
                description="For example @schafluzern"
            />
            <Step
                title="Post!"
                description="You're ready to go!"
            />
            <Step
                title="Share to Story"
                description="Share to your Instagram Story."
            />
        </div>
    </div>
    <h2>Caption</h2>
    <textarea
        class="instagram"
        v-model="text"
    ></textarea>
</template>

<style scoped>
textarea.instagram {
    height: 50ch;
}

.checklist {
    display: flex;
    flex-direction: column;
    gap: 1em;
}
</style>
