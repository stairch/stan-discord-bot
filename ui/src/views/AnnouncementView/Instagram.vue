<script setup lang="ts">
import { computed, watch, ref, type PropType, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import { api, type IServer, type IAnnouncement } from "../../api";
import { toUnicodeVariant } from "@/assets/toUnicodeVariant.js";
import CardPicker from "@/components/CardPicker.vue";

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
        return toUnicodeVariant(p1, "bold");
    };
    const boldText = fullText.replace(boldRegex, boldReplacer);

    // italic text is surrounded by _ or __
    const italicRegex = /__(.*?)__|_(.*?)_/g;
    const italicReplacer = (match: string, p1: string) => {
        return toUnicodeVariant(p1, "italic");
    };
    const italicText = boldText.replace(italicRegex, italicReplacer);

    return italicText;
};

onMounted(async () => {
    text.value = buildText();
});

watch(
    () => props.modelValue,
    (value) => {
        text.value = buildText();
    },
    { deep: true }
);
</script>

<template>
    <textarea
        class="instagram"
        v-model="text"
    ></textarea>
</template>

<style scoped>
textarea.instagram {
    height: 70ch;
}
</style>
