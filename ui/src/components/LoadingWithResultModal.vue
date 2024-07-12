<script setup lang="ts">
    import { ref } from "vue";
    import Modal from "@/components/Modal.vue";
    import Loader from "./Loader.vue";

    const isSuccess = ref(false);
    const isError = ref(false);
    const displayMessage = ref("");
    const showModal = ref(false);

    const onSuccess = (message?: string) => {
        isSuccess.value = true;
        displayMessage.value = message ?? "";
    };

    const onError = (message?: string) => {
        isError.value = true;
        displayMessage.value = message || "Something went wrong!";
    };

    const onLoading = () => {
        isSuccess.value = false;
        isError.value = false;
        displayMessage.value = "";
        showModal.value = true;
    };

    defineExpose({
        onSuccess,
        onError,
        onLoading,
    });
</script>

<template>
    <Modal v-model="showModal">
        <div class="content">
            <Loader v-if="!isSuccess && !isError" />
            <span
                class="error material-symbols-rounded"
                v-if="isError"
            >
                error
            </span>
            <span
                class="success material-symbols-rounded"
                v-if="isSuccess"
            >
                check_circle
            </span>
            <h3>{{ displayMessage }}</h3>
        </div>
    </Modal>
</template>

<style scoped>
    .content {
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 1em;
    }

    .error {
        color: var(--c-stair-burgundy);
    }

    .success {
        color: var(--c-stair-green);
    }

    .success,
    .error {
        font-size: 5rem;
        font-variation-settings: "FILL" 1, "wght" 400, "GRAD" 0, "opsz" 24;
        animation: bounceIn 0.5s;
    }

    @keyframes bounceIn {
        0% {
            transform: scale(0);
        }
        50% {
            transform: scale(1.2);
        }
        100% {
            transform: scale(1);
        }
    }
</style>
