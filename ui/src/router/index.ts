import { createRouter, createWebHistory } from "vue-router";

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
            path: "/",
            name: "home",
            component: () => import("../views/HomeView.vue"),
        },
        {
            path: "/announcements",
            name: "announcements",
            component: () => import("../views/Announcements/Overview.vue"),
        },
        {
            path: "/announcements/:id",
            name: "announcementWithId",
            component: () =>
                import("../views/Announcements/EditorView/index.vue"),
        },
        {
            path: "/discord/users",
            name: "discord-users",
            component: () =>
                import("../views/Discord/UserManagementView/index.vue"),
        },
        {
            path: "/discord/degree-programmes",
            name: "discord-degree-programmes",
            component: () => import("../views/Discord/DegreeProgrammes.vue"),
        },
        {
            path: "/promotion-schedule",
            name: "promotion-schedule",
            component: () => import("../views/PromotionSchedule/index.vue"),
        },
    ],
});

export default router;
