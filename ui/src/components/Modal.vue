<script setup lang="ts">
    import { ref, watch } from "vue";

    const props = defineProps({
        modelValue: {
            type: Boolean,
            default: false,
        },
    });

    const emit = defineEmits(["update:modelValue"]);

    const modal = ref<HTMLDialogElement | null>(null);

    const close = () => {
        modal.value?.close();
        emit("update:modelValue", false);
    };

    const open = () => {
        modal.value?.showModal();
        emit("update:modelValue", true);
    };

    watch(
        () => props.modelValue,
        (show) => {
            if (show) {
                open();
            } else {
                close();
            }
        }
    );
</script>

<template>
    <dialog
        class="modal"
        ref="modal"
        @close="close"
    >
        <span
            @click="close"
            class="material-symbols-rounded close"
            >close</span
        >
        <slot></slot>
    </dialog>
</template>

<style scoped>
    .modal {
        position: fixed;
        padding: 1em;

        &:not(:has(h1)) {
            padding-top: 3em;
        }

        & h1 {
            padding-top: 0;
        }

        & .close {
            position: absolute;
            top: 0.5em;
            right: 0.5em;
            color: var(--fg-text-muted);
            cursor: pointer;

            &:hover {
                color: var(--fg-text);
            }
        }
    }

    dialog {
        border: 1px solid var(--bg-muted);
        background: var(--bg-base);
        border-radius: 0.5em;
        outline: none;
        inset: 0;
        margin: auto;
        font-size: 1rem;
        min-width: 20vw;
        max-width: 80ch;
        padding: 0;

        &::backdrop {
            background-color: rgba(0, 0, 0, 0.5);
        }
    }
</style>
