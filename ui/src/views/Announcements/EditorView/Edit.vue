<script setup lang="ts">
    import { computed, ref, watch, type PropType } from "vue";
    import { api, type IServer, type IAnnouncement } from "@/api";
    import router from "@/router";
    import MultiFileMonaco from "@/components/MultiFileMonaco.vue";

    const props = defineProps({
        modelValue: { type: Object as PropType<IAnnouncement>, required: true },
    });
    const announcement = ref<IAnnouncement>(
        JSON.parse(JSON.stringify(props.modelValue))
    );

    watch(
        () => props.modelValue,
        (value) => {
            announcement.value = JSON.parse(JSON.stringify(value));
        },
        { deep: true }
    );

    const emit = defineEmits(["update:modelValue"]);

    const save = async () => {
        let result;

        if (announcement.value.id) {
            result = await api.announements.update(announcement.value);
        } else {
            result = await api.announements.create(announcement.value);
            router.push(`/announcements/${result.id}`);
        }
        emit("update:modelValue", result);
    };

    const saveDisabled = computed(() => {
        return (
            JSON.stringify(announcement.value) ===
            JSON.stringify(props.modelValue)
        );
    });

    const deleteAnnouncement = async () => {
        if (announcement.value.id) {
            await api.announements.delete(announcement.value.id);
            router.push("/announcements");
        }
    };
</script>

<template>
    <input
        type="text"
        v-model="announcement.title"
        placeholder="Title"
    />
    <MultiFileMonaco
        v-model="announcement.message"
        :filenames="{ de: 'German Content', en: 'English Content' }"
    />
    <div class="actions">
        <button
            @click="save"
            class="danger"
            :disabled="saveDisabled"
        >
            {{ announcement.id ? "Update" : "Create" }}
        </button>
        <button
            @click="deleteAnnouncement"
            class="danger"
            :disabled="!announcement.id"
        >
            Delete
        </button>
    </div>
</template>

<style scoped>
    .announcement {
        display: grid;
        grid-template-columns: 25ch 1fr;
        align-items: start;
        gap: 1em;
    }

    main {
        display: flex;
        flex-direction: column;
        gap: 1em;
    }

    aside {
        background-color: var(--bg-soft);
        border: 1px solid var(--bg-muted);
        border-radius: 0.5em;
    }

    .server {
        display: flex;
        align-items: center;
        padding: 1em;
        gap: 1em;
        border-radius: 0.5em;

        &.selected {
            background-color: var(--bg-muted);
        }

        & img {
            width: 2em;
            height: 2em;
            border-radius: 0.25em;
        }
    }

    .actions {
        display: flex;
        gap: 1em;
        justify-content: flex-end;
    }
</style>
