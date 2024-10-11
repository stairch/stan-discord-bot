<script setup lang="ts">
    import { ref, watchEffect } from "vue";

    const editing = ref(false);
    const input = ref<HTMLInputElement | null>(null);

    watchEffect(() => {
        if (editing.value) {
            input.value?.focus();
        }
    });

    defineProps({
        modelValue: String,
        placeholder: String,
    });
</script>
<template>
    <div
        class="editable-text"
        tabindex="0"
        @keyup.enter.stop="editing = !editing"
        @dblclick="editing = true"
    >
        <slot v-if="!editing && (modelValue || !placeholder)" />
        <div v-else>
            <input
                type="text"
                ref="input"
                :placeholder="placeholder"
                v-bind:value="modelValue"
                v-on:change="
                    $emit(
                        'update:modelValue',
                        ($event.target as HTMLInputElement).value
                    )
                "
                @blur="editing = false"
                @keyup.enter.stop="editing = false"
                @keyup.esc.stop="editing = false"
            />
        </div>
    </div>
</template>

<style scoped>
    .editable-text {
        padding: 0.1rem 0;
    }

    input {
        background: none;
        border: none;
        border-bottom: 1px solid var(--c-stair-green);
        border-radius: 0;
        padding: 0;
        width: 100%;
    }
</style>
