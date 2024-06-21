import { ref, computed } from "vue";
import { defineStore } from "pinia";
import { api, type IAnnouncementSummary } from "@/api";

export const useAnnouncementStore = defineStore("counter", () => {
    const announcements = ref<IAnnouncementSummary[]>([]);

    const update = async () => {
        announcements.value = await api.announements.getAll();
    };

    const create = async () => {
        const data = await api.announements.create({
            title: "",
            message: {
                de: "",
                en: "",
            }
        })
        return "/announcements/" + data.id;
    };

    update();

    return { announcements, update, create };
});
