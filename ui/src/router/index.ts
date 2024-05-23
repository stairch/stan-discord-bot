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
            path: "/announcement",
            name: "announcement",
            component: () => import("../views/AnnouncementView.vue"),
        },
        {
            path: "/announcement/:server",
            name: "announcementWithId",
            component: () => import("../views/AnnouncementView.vue"),
        },
    ],
});

export default router;
