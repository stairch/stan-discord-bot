<script setup lang="ts">
    import { ref, watch, type PropType } from "vue";
    import Monaco from "./Monaco.vue";

    const open = ref<string | null>(null);

    const props = defineProps({
        modelValue: {
            type: Object as PropType<Record<string, string>>,
            required: true,
        },
        filenames: {
            type: Object as PropType<Record<string, string>>,
            required: false,
        },
        language: {
            type: String,
            default: "markdown",
        },
    });

    watch(
        () => props.modelValue,
        (files) => {
            open.value = Object.keys(files)[0];
        },
        { immediate: true }
    );
</script>

<template>
    <div class="editor">
        <div class="ribbon">
            <div
                v-for="(content, file) in props.modelValue"
                class="file"
                :key="file"
                @click="open = file"
                :class="{ open: open === file }"
            >
                {{ props.filenames?.[file] ?? file }}
            </div>
        </div>
        <div class="file">
            <Monaco
                v-model="props.modelValue[open!]"
                :language="props.language"
            />
        </div>
    </div>
</template>

<style scoped>
    .editor {
        display: flex;
        flex-direction: column;
        border-radius: 1em;
        overflow: hidden;
        background-color: var(--bg-soft);
        border: 1px solid var(--bg-muted);
    }

    .ribbon {
        display: flex;
        padding: 0.5em;
        gap: 0.5em;
        background-color: var(--bg-soft);

        .file {
            border-radius: 0.5em;
            padding: 0.5em 2em;
            cursor: pointer;
            border: 1px solid transparent;

            &.open {
                background-color: var(--bg-base);
                border: 1px solid var(--bg-muted);
            }
        }
    }
</style>
