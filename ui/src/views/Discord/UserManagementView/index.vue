<script lang="ts" setup>
    import { api, type IStudentStats } from "@/api";
    import { onMounted, ref } from "vue";
    import LoadingWithResultModal from "@/components/LoadingWithResultModal.vue";

    const modal = ref<InstanceType<typeof LoadingWithResultModal> | null>(null);

    const students = ref<IStudentStats | null>(null);

    const uploadCsv = async (type: "students" | "modules") => {
        const input = document.createElement("input");
        input.type = "file";
        input.accept = ".csv";
        input.onchange = async () => {
            modal.value?.onLoading();

            if (!input.files || input.files.length === 0) {
                return;
            }

            const file = input.files[0];
            const data = await file.text();
            const header = data.split("\n")[0];
            let error = null;

            if (type === "students") {
                if (!header.includes("Nachname")) {
                    modal.value?.onError("This format is unsupported.");
                    return;
                }
                error = await api.db.updateStudents(data);
            } else if (type === "modules") {
                if (!header.includes("Modultyp")) {
                    modal.value?.onError("This format is unsupported.");
                    return;
                }
                error = await api.db.updateModules(data);
            }

            if (!error) {
                modal.value?.onSuccess("Successfully updated!");
            } else {
                modal.value?.onError(error);
            }
        };
        input.click();
    };

    onMounted(async () => {
        students.value = await api.db.students();
    });
</script>
<template>
    <main>
        <LoadingWithResultModal ref="modal" />
        <h1>User Management</h1>
        <div class="user-management">
            <div
                class="stats"
                v-if="students"
            >
                <div class="card">
                    <h1>{{ students.enrolled }}</h1>
                    <p>HSLU-I Students</p>
                </div>
                <div class="card">
                    <h1>{{ students.discord.students }}</h1>
                    <p>Students on Discord</p>
                </div>
                <div class="card">
                    <h1>{{ students.discord.graduates }}</h1>
                    <p>Graduates on Discord</p>
                </div>
            </div>
            <h2>Configuration</h2>
            <div class="config">
                <div class="config-option">
                    <div class="info">
                        <h3>Update Students</h3>
                        <p>
                            Update the list of students that is provided by the
                            HSLU administration.
                        </p>
                    </div>
                    <button
                        class="secondary"
                        @click="uploadCsv('students')"
                    >
                        <span class="material-symbols-rounded"
                            >file_upload</span
                        >
                        Update
                    </button>
                </div>
                <div class="config-option">
                    <div class="info">
                        <h3>Update Modules</h3>
                        <p>
                            Update the list of modules that is provided by the
                            HSLU administration.
                        </p>
                    </div>
                    <button
                        class="secondary"
                        @click="uploadCsv('modules')"
                    >
                        <span class="material-symbols-rounded"
                            >file_upload</span
                        >
                        Update
                    </button>
                </div>
                <div class="config-option">
                    <div class="info">
                        <h3>Edit Degree Programmes</h3>
                        <p>
                            Edit the mapping between the HSLU degree programmes
                            and the Discord roles & channels.
                        </p>
                    </div>
                    <router-link
                        to="/discord/degree-programmes"
                        class="secondary"
                    >
                        <button class="secondary">
                            <span class="material-symbols-rounded">edit</span>
                            Edit
                        </button>
                    </router-link>
                </div>
            </div>
        </div>
    </main>
</template>
<style scoped>
    .user-management {
        display: flex;
        flex-direction: column;
        gap: 1em;
    }

    .stats {
        display: flex;
        flex-direction: row;
        flex-wrap: wrap;
        gap: 1em;

        .card {
            min-width: 30ch;
            flex: 1;
        }
    }

    .config {
        display: flex;
        flex-direction: column;
        gap: 1em;
    }

    .config-option {
        display: grid;
        grid-template-columns: 1fr max-content;
        align-items: center;
        gap: 1em;

        &:not(:last-child) {
            padding-bottom: 1em;
            border-bottom: 1px solid var(--bg-muted);
        }
    }
</style>
