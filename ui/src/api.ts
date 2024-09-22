import { useAnnouncementStore } from "./stores/announcements";

export interface IServer {
    id: string;
    name: string;
    picture: string;
}

export interface ISchedule {
    scope: AnnouncementScope;
    type: AnnouncementType;
    persona: string;
    server: string;
    days: number[];
    time: string;
}

export interface IAnnouncement {
    id?: number;
    title: string;
    message: {
        en: string;
        de: string;
    };
    author?: string;
    lastModified?: string;
}

export interface IAnnouncementSummary {
    id: number;
    title: string;
    author: string;
    lastModified: string;
}

export interface IStudentStats {
    enrolled: number;
    discord: {
        students: number;
        graduates: number;
    };
}

export interface IDegreeProgramme {
    id: string;
    category: string;
    role: string;
    colour: string;
    channel: string;
}

type AnnouncementScope = "discord" | "telegram";
export const ANNOUNCEMENT_TYPES = ["test", "stair", "non-stair", "server"];
type AnnouncementType = (typeof ANNOUNCEMENT_TYPES)[number];

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
        schedule: {
            async get(announcementId: number): Promise<ISchedule[]> {
                return fetch(
                    `/api/announcements/${announcementId}/schedules`
                ).then((x) => x.json());
            },
            async update(
                announcementId: number,
                schedules: ISchedule[]
            ): Promise<string | null> {
                const res = await fetch(
                    `/api/announcements/${announcementId}/schedules`,
                    {
                        method: "PUT",
                        headers: {
                            "Content-Type": "application/json",
                        },
                        body: JSON.stringify(schedules),
                    }
                );
                if (res.ok) {
                    return null;
                }
                const data = await res.text();
                console.error(data);
                return data;
            },
        },
        async publish(
            id: number,
            scope: AnnouncementScope,
            server: string,
            type: AnnouncementType,
            persona: string,
            image?: File
        ): Promise<string | null> {
            const res = await fetch(`/api/announcements/${id}/publish`, {
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
            if (res.ok) {
                return null;
            }
            const data = await res.text();
            console.error(data);
            return data;
        },
        async discordServers(): Promise<IServer[]> {
            return fetch(`/api/announcements/discord/servers`).then((res) =>
                res.json()
            );
        },
        async telegramChats(): Promise<IServer[]> {
            return fetch(`/api/announcements/telegram/chats`).then((res) =>
                res.json()
            );
        },
        async personas(): Promise<string[]> {
            return fetch(`/api/announcements/personas`).then((res) =>
                res.json()
            );
        },
    },
    db: {
        async students(): Promise<IStudentStats> {
            return fetch("/api/students").then((res) => res.json());
        },
        async updateStudents(csvAsString: string): Promise<string | null> {
            const res = await fetch("/api/students", {
                method: "PUT",
                headers: {
                    "Content-Type": "application/xml",
                },
                body: csvAsString,
            });
            if (res.ok) {
                return null;
            }
            return await res.text();
        },
        async updateModules(csvAsString: string): Promise<string | null> {
            const res = await fetch("/api/modules", {
                method: "PUT",
                headers: {
                    "Content-Type": "application/xml",
                },
                body: csvAsString,
            });
            if (res.ok) {
                return null;
            }
            return await res.text();
        },
        async getDegreeProgrammes(): Promise<IDegreeProgramme[]> {
            return fetch("/api/degree-programmes").then((res) => res.json());
        },
        async updateDegreeProgrammes(
            degreeProgrammes: IDegreeProgramme[]
        ): Promise<void> {
            await fetch("/api/degree-programmes", {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(degreeProgrammes),
            });
        },
    },
};
