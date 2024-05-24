<script setup lang="ts">
import { ref, type PropType } from "vue";

interface IOption {
    value: string;
    label: string;
}

defineProps({
    options: {
        type: Array as PropType<IOption[]>,
        required: true,
    },
    modelValue: {
        type: String,
        required: true,
    },
});
</script>
<template>
    <div class="options">
        <div
            class="card"
            :class="{ selected: modelValue === option.value }"
            v-for="option in options"
            :key="option.value"
            @click="$emit('update:modelValue', option.value)"
        >
            <p>{{ option.label }}</p>
        </div>
    </div>
</template>

<style scoped>
.options {
    display: flex;
    flex-wrap: wrap;
    gap: 1em;
}

.card {
    cursor: pointer;
}

.selected {
    background-color: var(--c-stair-lime);
    color: var(--c-white-1);
    animation: bounce 0.3s ease-in-out;
}

@keyframes bounce {
    0% {
        transform: scale(1);
    }
    50% {
        transform: scale(0.98);
    }
    100% {
        transform: scale(1);
    }
}

p {
    text-transform: capitalize;
}
</style>
