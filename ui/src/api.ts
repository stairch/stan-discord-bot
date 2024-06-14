import { useAnnouncementStore } from "./stores/announcements";

export interface IServer {
    id: string;
    name: string;
    picture: string;
}

export interface IAnnouncement {
    id?: number;
    title: string;
    message: {
        en: string;
        de: string;
    };
}

export interface IAnnouncementSummary {
    id: number;
    title: string;
}

type AnnouncementScope = "discord" | "telegram";
export const ANNOUNCEMENT_TYPES = ["stair", "non-stair", "server", "test"]
type AnnouncementType = typeof ANNOUNCEMENT_TYPES[number];

const toBase64 = async (file?: File): Promise<string | undefined> => {
    if (!file) {
        return undefined;
    }
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => {
            const base64 = reader.result as string;
            resolve(base64.replace(/^data:image\/[a-z]+;base64,/, ""));
        };
        reader.onerror = reject;
        reader.readAsDataURL(file);
    });
};

const forceReloadAnnouncements = () => {
    useAnnouncementStore().update();
};

export const api = {
    announements: {
        async getAll(): Promise<IAnnouncementSummary[]> {
            return fetch("/api/announcements").then((res) => res.json());
        },
        async create(announcement: IAnnouncement): Promise<IAnnouncement> {
            const res = await fetch("/api/announcements", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(announcement),
            });
            const data = await res.json();
            forceReloadAnnouncements();
            return data;
        },
        async get(id: number): Promise<IAnnouncement> {
            return fetch(`/api/announcements/${id}`).then((res) => res.json());
        },
        async update(announcement: IAnnouncement): Promise<IAnnouncement> {
            const res = await fetch(`/api/announcements/${announcement.id}`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(announcement),
            });
            const data = await res.json();
            forceReloadAnnouncements();
            return data;
        },
        async delete(id: number): Promise<void> {
            await fetch(`/api/announcements/${id}`, {
                method: "DELETE",
            }).then(forceReloadAnnouncements);
        },
        async getTypes(): Promise<string[]> {
            return fetch(`/api/announcements/types`).then((res) => res.json());
        },
        async publish(
            id: number,
            scope: AnnouncementScope,
            server: string,
            type: AnnouncementType,
            persona: string,
            image?: File
        ): Promise<void> {
            fetch(`/api/announcements/${id}/publish`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    id,
                    scope,
                    server,
                    type,
                    persona,
                    image: await toBase64(image),
                }),
            });
        },
        async discordServers(): Promise<IServer[]> {
            return fetch(`/api/announcements/discord/servers`).then((res) =>
                res.json()
            );
        },
        async personas(): Promise<string[]> {
            return fetch(`/api/announcements/personas`).then((res) =>
                res.json()
            );
        },
    },
};
