<script setup lang="ts">
    import { computed, ref } from "vue";

    const props = defineProps<{
        modelValue: Date;
    }>();

    const emit = defineEmits<{
        (e: "update:modelValue", value: Date): void;
    }>();

    const toIsoString = (date: Date) => {
        return date.toISOString().split("T")[0];
    };

    const date = computed<string>({
        get: () => toIsoString(props.modelValue),
        set: (value: Date | string) => {
            const newDate = new Date(value);
            emit("update:modelValue", newDate);
        },
    });

    const dateInput = ref<HTMLInputElement>();

    const showPicker = () => {
        if (dateInput.value) {
            dateInput.value.showPicker();
        }
    };

    const setDate = () => {
        if (dateInput.value) {
            const newDate = new Date(dateInput.value.value);
            emit("update:modelValue", newDate);
        }
    };
</script>
<template>
    <div
        class="date"
        @click="showPicker"
    >
        <div class="day">
            {{ modelValue.getDate() }}
        </div>
        <div class="month">
            {{
                modelValue.toLocaleString("default", {
                    month: "short",
                })
            }}
        </div>
        <input
            type="date"
            v-show="false"
            ref="dateInput"
            :value="date"
            @change="setDate"
        />
    </div>
</template>
<style scoped>
    .date {
        display: flex;
        flex-direction: column;
        align-items: center;
        position: inherit;
        cursor: pointer;

        .day {
            font-size: 1.5rem;
            font-weight: bold;
        }

        .month {
            font-size: 0.7rem;
            text-transform: uppercase;
            color: var(--text-secondary);
        }
    }
</style>
