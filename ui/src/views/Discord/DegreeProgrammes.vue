<script setup lang="ts">
    import { api, type IDegreeProgramme } from "@/api";
    import EditableText from "@/components/EditableText.vue";
    import { onMounted, ref, computed } from "vue";

    const programmes = ref<IDegreeProgramme[]>([]);
    const initial = ref<string>("");

    const fetchData = async () => {
        programmes.value = await api.db.getDegreeProgrammes();
        initial.value = JSON.stringify(programmes.value);
    };

    const updateData = async () => {
        await api.db.updateDegreeProgrammes(programmes.value);
        await fetchData();
    };

    onMounted(fetchData);

    const changed = computed(() => {
        return JSON.stringify(programmes.value) !== initial.value;
    });
</script>
<template>
    <main>
        <h1>Degree Programmes</h1>
        <div class="actions">
            <button
                @click="updateData"
                :disabled="!changed"
            >
                <span class="material-symbols-rounded">save</span>
                Save
            </button>
        </div>
        <div class="programmes">
            <div class="programme">
                <span class="swatch"></span>
                <p>ID</p>
                <span>Category</span>
                <span>Channel</span>
                <span>Role</span>
                <span class="delete"></span>
            </div>
            <div
                v-for="programme in programmes"
                :key="programme.id"
                class="programme"
            >
                <div class="swatch-wrapper">
                    <input
                        type="color"
                        class="swatch"
                        v-model="programme.colour"
                    />
                </div>
                <EditableText
                    placeholder="'Anlassnummer' from students db"
                    v-model="programme.id"
                >
                    <p>{{ programme.id }}</p>
                </EditableText>
                <EditableText
                    placeholder="Discord channel category"
                    v-model="programme.category"
                >
                    <span>{{ programme.category }}</span>
                </EditableText>
                <EditableText
                    placeholder="Discord channel name"
                    v-model="programme.channel"
                >
                    <span>{{ programme.channel }}</span>
                </EditableText>
                <EditableText
                    placeholder="Discord role name"
                    v-model="programme.role"
                >
                    <span>{{ programme.role }}</span>
                </EditableText>
                <span
                    @click="programmes.splice(programmes.indexOf(programme), 1)"
                    class="delete material-symbols-rounded"
                >
                    delete
                </span>
            </div>
            <div
                class="add-programme"
                @click="
                    programmes.push({
                        id: '',
                        category: '',
                        channel: '',
                        role: '',
                        colour: '#000000',
                    })
                "
            >
                <span class="material-symbols-rounded">add</span>
                Add Programme
            </div>
        </div>
    </main>
</template>
<style scoped>
    .programmes {
        display: flex;
        flex-direction: column;
    }

    .actions {
        display: flex;
        justify-content: flex-end;
        gap: 1em;
    }

    .programme {
        display: flex;
        flex-direction: row;
        flex-wrap: wrap;
        align-items: center;
        gap: 1em;
        padding: 1em;

        :not(:first-child):not(:last-child) {
            flex: 1;
            min-width: 10ch;
        }

        &:not(:last-child) {
            border-bottom: 1px solid var(--bg-muted);
        }

        @media screen and (max-width: 768px) {
            gap: 0.5em;

            :not(:first-child) {
                min-width: 5ch;
            }
        }
    }

    .delete {
        font-size: 1rem;
        width: 1rem;
    }

    .swatch {
        display: block;
        width: 10px;
        border-radius: 50%;
        padding: 0;
        margin: 0;
        border: none;
    }

    .swatch-wrapper {
        height: 10px;
        width: 10px;
        overflow: hidden;
        border-radius: 50%;
        display: inline-flex;
        align-items: center;
        position: relative;
    }
    .swatch-wrapper input[type="color"] {
        position: absolute;
        height: 4em;
        width: 4em;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        overflow: hidden;
        border: none;
        margin: 0;
        padding: 0;
    }

    .add-programme {
        width: 100%;
        border-radius: 0.5em;
        display: flex;
        flex-direction: row;
        align-items: center;
        gap: 1em;
        padding: 1em;
        cursor: pointer;

        &:hover {
            background-color: var(--c-stair-green-20);
            color: var(--c-stair-green);
        }
    }
</style>
