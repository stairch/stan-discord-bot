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
            component: () => import("../views/AnnouncementView/index.vue"),
        },
        {
            path: "/announcements/:id",
            name: "announcementWithId",
            component: () => import("../views/AnnouncementView/index.vue"),
        },
        {
            path: "/discord/users",
            name: "discord-users",
            component: () => import("../views/Discord/UserManagementView/index.vue"),
        },
    ],
});

export default router;
