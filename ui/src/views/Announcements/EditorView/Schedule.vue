<script setup lang="ts">
    import { computed, ref, type PropType, onMounted } from "vue";
    import {
        api,
        ANNOUNCEMENT_TYPES,
        type IServer,
        type IAnnouncement,
        type ISchedule,
    } from "@/api";
    import Loader from "@/components/Loader.vue";

    const telegramServers = ref<IServer[]>([]);
    const discordServers = ref<IServer[]>([]);
    const personas = ref<string[]>([]);

    const schedules = ref<ISchedule[]>([]);

    const loading = ref(true);

    const props = defineProps({
        modelValue: { type: Object as PropType<IAnnouncement>, required: true },
    });
    const announcement = computed(() => ({ ...props.modelValue }));

    const days = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

    onMounted(async () => {
        discordServers.value = await api.announements.discordServers();
        telegramServers.value = await api.announements.telegramChats();
        personas.value = await api.announements.personas();

        schedules.value = await api.announements.schedule.get(
            announcement.value.id!
        );

        loading.value = false;
    });

    const toggleDay = (schedule: ISchedule, day: number) => {
        if (schedule.days.includes(day)) {
            schedule.days = schedule.days.filter((x) => x != day);
        } else {
            schedule.days.push(day);
            schedule.days.sort();
        }
    };

    const addSchedule = () => {
        schedules.value.push({
            scope: "discord",
            persona: personas.value[0],
            type: ANNOUNCEMENT_TYPES[0],
            server: discordServers.value[0].id,
            time: "12:00:00",
            days: [],
        });
    };

    const save = async () => {
        loading.value = true;
        await api.announements.schedule.update(
            announcement.value.id!,
            schedules.value
        );
        schedules.value = await api.announements.schedule.get(
            announcement.value.id!
        );
        loading.value = false;
    };
</script>

<template>
    <div
        class="loading"
        v-if="loading"
    >
        <Loader />
    </div>
    <div
        v-else
        class="schedules"
    >
        <div
            class="schedule"
            v-for="(schedule, i) in schedules"
        >
            <span
                class="material-symbols-rounded delete"
                @click="schedules.splice(i, 1)"
                title="Delete"
            >
                delete
            </span>
            <div class="row">
                <div class="dropdown">
                    <label>Platform</label>
                    <select v-model="schedule.scope">
                        <option
                            v-for="s in ['discord', 'telegram']"
                            :key="s"
                            :value="s"
                        >
                            {{ s }}
                        </option>
                    </select>
                </div>
                <div class="dropdown">
                    <label>
                        On
                        {{ schedule.scope == "discord" ? "Server" : "Channel" }}
                    </label>
                    <select
                        v-model="schedule.server"
                        v-if="schedule.scope == 'discord'"
                    >
                        <option
                            v-for="s in discordServers"
                            :key="s.id"
                            :value="s.id"
                        >
                            {{ s.name }}
                        </option>
                    </select>
                    <select
                        v-model="schedule.server"
                        v-else-if="schedule.scope == 'telegram'"
                    >
                        <option
                            v-for="s in telegramServers"
                            :key="s.id"
                            :value="s.id"
                        >
                            {{ s.name }}
                        </option>
                    </select>
                </div>
                <template v-if="schedule.scope == 'discord'">
                    <div class="dropdown">
                        <label>As</label>
                        <select v-model="schedule.type">
                            <option
                                v-for="t in ANNOUNCEMENT_TYPES"
                                :key="t"
                                :value="t"
                            >
                                {{ t }}
                            </option>
                        </select>
                    </div>
                    <div class="dropdown">
                        <label>Post as</label>
                        <select v-model="schedule.persona">
                            <option
                                v-for="t in personas"
                                :key="t"
                                :value="t"
                            >
                                {{ t }}
                            </option>
                        </select>
                    </div>
                </template>
            </div>
            <div class="row">
                <div class="dropdown">
                    <label>At Time (UTC)</label>
                    <input
                        type="time"
                        v-model="schedule.time"
                    />
                </div>
                <div class="dropdown">
                    <label>On Weekdays</label>
                    <div class="days">
                        <div
                            v-for="(day, index) in days"
                            class="day"
                            :class="{
                                selected: schedule.days.includes(index),
                            }"
                            @click="toggleDay(schedule, index)"
                        >
                            {{ day }}
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div
            @click="addSchedule"
            class="add-schedule"
        >
            <span class="material-symbols-rounded">add</span>
            Add schedule
        </div>
    </div>
    <div class="actions">
        <button @click="save">Save</button>
    </div>
</template>

<style scoped>
    .loading {
        padding: 2em;
        width: 100%;
        display: flex;
        flex-direction: row;
        align-items: center;
        justify-content: center;
    }

    .inputs {
        display: flex;
        gap: 1em;
        align-items: center;
    }

    .align-right {
        margin-left: auto;
    }

    .dropdown {
        display: flex;
        flex-direction: column;
        gap: 0.5em;
    }

    .days {
        display: flex;
        flex-direction: row;
        gap: 0.5em;
    }

    .day {
        padding: 8px 12px;
        color: var(--fg-text);
        background: var(--bg-soft);
        border: 1px solid var(--bg-muted);
        border-radius: 4px;
        cursor: pointer;

        &.selected {
            color: var(--c-white-1);
            background: var(--c-stair-green);
        }
    }

    .schedules {
        display: flex;
        flex-direction: column;
        gap: 1em;
    }

    .schedule {
        color: var(--fg-text);
        background: var(--bg-soft);
        border: 1px solid var(--bg-muted);
        border-radius: 4px;
        padding: 8px 12px;

        display: flex;
        flex-direction: column;
        gap: 2em;
        position: relative;

        & .row {
            display: flex;
            flex-direction: row;
            gap: 1em;
        }

        .delete {
            position: absolute;
            right: 0.5em;
            top: 0.5em;
            cursor: pointer;
            background: var(--bg-base);
            padding: 0.25em;
            font-size: 1.8em;
            border: 1px solid var(--bg-muted);
            border-radius: 0.25em;

            &:hover {
                color: var(--c-accent);
                border-color: var(--c-accent);
            }
        }
    }

    .add-schedule {
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
