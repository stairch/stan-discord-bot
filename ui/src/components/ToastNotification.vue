<script setup lang="ts">
    import { ref } from "vue";

    interface INotification {
        message: string;
        icon?: string;
        style?: "info" | "success" | "error";
        duration?: number;
    }
    const showNotification = ref<INotification | null>(null);

    const showToast = (notification: INotification) => {
        notification.duration = notification.duration || 5000;
        notification.style = notification.style || "info";
        notification.icon = notification.icon || "info";

        if (notification.duration > 0) {
            setTimeout(() => {
                showNotification.value = null;
            }, notification.duration);
        }

        showNotification.value = notification;
    };

    defineExpose({
        showToast,
    });
</script>
<template>
    <div
        v-if="showNotification"
        class="toast"
        :class="showNotification.style"
        :style="{
            '--duration': showNotification.duration + 'ms',
        }"
    >
        <span class="material-symbols-rounded">{{
            showNotification.icon
        }}</span>
        {{ showNotification.message }}
    </div>
</template>

<style scoped>
    .toast {
        position: fixed;
        bottom: 2rem;
        left: 50%;
        transform: translateX(-50%);

        display: flex;
        align-items: center;
        gap: 0.5em;

        background-color: var(--bg-base);
        color: var(--text-main);
        border: 1px solid var(--border-base);
        padding: 1rem;
        border-radius: 0.5rem;
        border: 1px solid var(--bg-mute);
        z-index: 1000;
        box-shadow: 0 0 1rem rgba(0, 0, 0, 0.25);
        transition: opacity 0.5s;
        opacity: 1;
        animation: fadeInOut 5s;

        &.success {
            background-color: var(--c-stair-green);
            color: var(--c-white-1);
        }

        &.error {
            background-color: var(--c-stair-burgundy);
            color: var(--c-white-1);
        }

        & span {
            font-size: 1rem;
        }
    }

    @keyframes fadeInOut {
        0% {
            opacity: 0;
            transform: translateY(100%) translateX(-50%);
        }
        10% {
            opacity: 1;
            transform: translateY(0) translateX(-50%);
        }
        90% {
            opacity: 1;
            transform: translateX(-50%);
        }
        100% {
            opacity: 0;
            transform: translateX(-50%);
        }
    }
</style>
