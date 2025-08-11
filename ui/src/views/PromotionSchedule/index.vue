<script setup lang="ts">
    import DatePicker from "@/components/DatePicker.vue";
    import EditableText from "@/components/EditableText.vue";
    import { computed, ref } from "vue";

    const estimateDefaultFirstDay = () => {
        // take either a monday in the middle of september or a monday in the middle of february, whichever is closer
        const now = new Date();
        const year = now.getFullYear();

        const thisFebruary = new Date(year, 1, 15); // 15 February
        const september = new Date(year, 8, 15); // 15 September
        const nextFebruary = new Date(year + 1, 1, 15); // next year's 15 February

        const diffThisFebruary = Math.abs(
            now.getTime() - thisFebruary.getTime()
        );
        const diffNextFebruary = Math.abs(
            now.getTime() - nextFebruary.getTime()
        );
        const february =
            diffThisFebruary < diffNextFebruary ? thisFebruary : nextFebruary;
        const diffFebruary = Math.abs(now.getTime() - february.getTime());

        const diffSeptember = Math.abs(now.getTime() - september.getTime());

        const firstDay = diffSeptember < diffFebruary ? september : february;
        firstDay.setDate(firstDay.getDate() - firstDay.getDay() + 1); // move to monday
        return firstDay;
    };
    const firstDay = ref(estimateDefaultFirstDay());
    const configurableFirstDay = computed({
        get: () => firstDay.value.toISOString().split("T")[0],
        set: (value: Date | string) => {
            if (value instanceof Date) {
                firstDay.value = value;
            } else {
                firstDay.value = new Date(value);
            }
        },
    });

    /**
     * Converts a school week and weekday to a Date object.
     * @param schoolweek The school week number (1-based).
     * @param weekday The weekday number (0 for Sunday, 1 for Monday, ..., 6 for Saturday).
     * @returns A Date object representing the date of the given school week and weekday.
     */
    const swToDay = (schoolweek: number, weekday: number) => {
        const date = firstDay.value;

        // move to sw
        date.setDate(date.getDate() + (schoolweek - 1) * 7);
        // set day
        if (weekday === 0) {
            // sunday
            date.setDate(date.getDate() + 6);
        } else {
            // firstDay is monday
            date.setDate(date.getDate() + weekday - 1);
        }

        return date;
    };

    interface ReminderConfig {
        count: number; // number of reminders
        dayBefore?: boolean; // whether to add a reminder the day before the event
    }
    const DEFAULT_REMINDER: ReminderConfig = {
        count: 2,
        dayBefore: true,
    };
    interface Event {
        name: string; // name of the event
        responsible: string; // person responsible for the event
        date: Date; // date of the event
        reminders?: ReminderConfig; // optional reminders configuration
    }
    interface EventPromoSchedule {
        eventStory: Date; // date of the event story
        post: Date; // date of the post
        reminders: Date[]; // dates of the reminders
    }
    interface Task {
        name: string; // name of the task
        responsible: string; // person responsible for the task
        due: Date; // due date of the task
    }

    const schedulePromo = (
        eventDate: Date,
        reminders: ReminderConfig | null = DEFAULT_REMINDER
    ) => {
        reminders ??= DEFAULT_REMINDER;
        reminders.dayBefore ??= DEFAULT_REMINDER.dayBefore;

        const RANGE = 14;
        const interval = Math.floor(RANGE / (reminders.count + 1));

        const schedule: EventPromoSchedule = {
            eventStory: new Date(eventDate),
            post: new Date(eventDate),
            reminders: [],
        };
        schedule.post.setDate(schedule.post.getDate() - RANGE);

        if (reminders.dayBefore) {
            const reminder = new Date(eventDate);
            reminder.setDate(reminder.getDate() - 1);
            schedule.reminders.push(reminder);
        }

        for (let i = 0; i < reminders.count; i++) {
            const reminder = new Date(eventDate);
            reminder.setDate(reminder.getDate() - interval * (i + 1));
            schedule.reminders.push(reminder);
        }

        return schedule;
    };

    const createTasks = (events: Event[]) => {
        const tasks: Task[] = [];
        for (const event of events) {
            const schedule = schedulePromo(event.date, event.reminders);
            tasks.push({
                name: event.name + " Post",
                responsible: event.responsible,
                due: schedule.post,
            });
            for (let i = 0; i < schedule.reminders.length; i++) {
                let name = event.name + " Reminder";
                if (schedule.reminders.length > 1) {
                    name += ` ${i + 1}`;
                }

                tasks.push({
                    name: name,
                    responsible: event.responsible,
                    due: schedule.reminders[schedule.reminders.length - i - 1],
                });
            }
            tasks.push({
                name: event.name + " Event Story",
                responsible: event.responsible,
                due: schedule.eventStory,
            });
        }

        tasks.sort((a, b) => {
            return a.due.getTime() - b.due.getTime();
        });

        return tasks;
    };

    const addEvent = () => {
        const lastDate =
            events.value[events.value.length - 1]?.date || new Date();
        const nextDate = new Date(lastDate);
        nextDate.setDate(nextDate.getDate() + 7); // add one week
        events.value.push({
            name: "New Event",
            responsible: "",
            date: nextDate,
        });
    };

    const events = ref<Event[]>([]);
    const sortedEvents = computed({
        get: () => {
            return events.value.sort(
                (a, b) => a.date.getTime() - b.date.getTime()
            );
        },
        set: (value: Event[]) => {
            events.value = value;
        },
    });

    const tasks = computed(() => createTasks(sortedEvents.value));
    const tasksByMonth = computed(() => {
        const tasksByMonth: Record<string, Task[]> = {};
        for (const task of tasks.value) {
            const month = task.due.toLocaleString("default", { month: "long" });
            if (!tasksByMonth[month]) {
                tasksByMonth[month] = [];
            }
            tasksByMonth[month].push(task);
        }
        return tasksByMonth;
    });
</script>
<template>
    <main>
        <div class="promotion-scheduler">
            <h1>Promotion Schedule</h1>
            <div
                v-if="false"
                class="config"
            >
                <input
                    type="date"
                    v-model="configurableFirstDay"
                    class="first-day"
                    title="First Day of Semester"
                />
            </div>
            <div class="hstack">
                <aside class="events">
                    <h2>Upcoming Events</h2>
                    <div class="list">
                        <div
                            class="event"
                            v-for="event in events"
                            :key="event.name"
                        >
                            <div class="left">
                                <DatePicker v-model="event.date" />
                                <div class="vstack">
                                    <EditableText
                                        placeholder="Event Name"
                                        v-model="event.name"
                                    >
                                        <strong>{{ event.name }}</strong>
                                    </EditableText>
                                    <EditableText
                                        placeholder="Responsible Person"
                                        v-model="event.responsible"
                                    >
                                        <span class="responsible">
                                            {{ event.responsible }}
                                        </span>
                                    </EditableText>
                                </div>
                            </div>
                            <span
                                class="material-symbols-rounded delete"
                                @click="events.splice(events.indexOf(event), 1)"
                                title="Delete"
                            >
                                delete
                            </span>
                        </div>
                        <div
                            @click="addEvent"
                            class="add-event"
                        >
                            <span class="material-symbols-rounded">add</span>
                            Add event
                        </div>
                    </div>
                </aside>
                <div class="schedule">
                    <h2>Promotion Schedule</h2>
                    <div class="promo">
                        <div
                            class="month"
                            v-for="(tasks, month) in tasksByMonth"
                        >
                            <span class="caption">{{ month }}</span>
                            <div
                                class="event"
                                v-for="task in tasks"
                                :key="task.name + task.due.toISOString()"
                            >
                                <DatePicker v-model="task.due" />
                                <div class="vstack">
                                    <strong>{{ task.name }}</strong>
                                    <span class="responsible">
                                        {{ task.responsible }}
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </main>
</template>

<style scoped>
    .hstack {
        display: grid;
        grid-template-columns: 1fr 2fr;
        gap: 3em;
        height: calc(100% - 8em);
        overflow: hidden;
    }

    .promotion-scheduler {
        overflow: clip;
        height: calc(100svh - 10em);
    }

    .add-event {
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

    .vstack {
        display: flex;
        flex-direction: column;
        gap: 0.25rem;
    }

    .event {
        display: flex;
        flex-direction: row;
        align-items: center;
        gap: 1em;
        padding: 0.5em 1.5em;
        border-radius: 0.5em;

        .left {
            flex: 1;
            font-size: 1rem;
            display: grid;
            grid-template-columns: 2rem auto;
            gap: 0.5rem;
            align-items: center;

            .responsible {
                font-size: 0.875rem;
                color: var(--fg-text-muted);
            }
        }

        &:hover {
            background-color: var(--bg-muted);
        }

        &:not(:hover) .delete {
            display: none;
        }

        .delete {
            cursor: pointer;
            font-size: 1.5rem;

            &:hover {
                color: var(--c-stair-burgundy);
            }
        }
    }

    .events,
    .schedule {
        overflow: auto;
        position: relative;

        & h2 {
            position: sticky;
            top: 0;
            padding: 1em 0 0.5em 0;
            background-color: var(--bg-base);
        }
    }

    .promo {
        display: flex;
        flex-direction: column;
        gap: 1em;
        margin-top: 1em;
    }
</style>
