import { ref, computed } from "vue";
import { defineStore } from "pinia";
import { api, type IAnnouncementSummary } from "@/api";

export const useAnnouncementStore = defineStore("counter", () => {
    const announcements = ref<IAnnouncementSummary[]>([]);

    const update = async () => {
        announcements.value = await api.announements.getAll();
    };

    update();

    return { announcements, update };
});
