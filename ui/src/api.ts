import { useAnnouncementStore } from "./stores/announcements";

export interface IServer {
    id: string;
    name: string;
    picture: string;
}

export interface ISchedule {
    scope: AnnouncementScope;
    type: string;
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

export type IPersonaDefinition = Record<
    string,
    {
        avatar: string;
    }
>;

export type IAnnouncementTypesDefinition = Record<
    string,
    {
        name: string;
        role: string;
    }
>;

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
        async search(
            query: string,
            byMe: boolean = false,
            timeRange?: {
                start: Date;
                end: Date;
            },
            limit: number = 10,
            offset: number = 0
        ): Promise<{ items: IAnnouncementSummary[]; totalCount: number }> {
            const params = new URLSearchParams();
            params.append("query", query);
            params.append("author", byMe ? "me" : "");
            params.append("start", timeRange?.start?.toISOString() || "");
            params.append("end", timeRange?.end?.toISOString() || "");
            params.append("limit", limit.toString());
            params.append("offset", offset.toString());
            const res = await fetch(`/api/announcements?${params}`);
            const data = await res.json();
            return {
                items: data,
                totalCount: Number(res.headers.get("X-Total-Count") || 0),
            };
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
        async types(): Promise<string[]> {
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
            type: string,
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
    commonSources: {
        _cache: new Map<string, any>(),
        personas: {
            _base: "/common/personas",
            _cacheKey: "personas",
            async definition(): Promise<IPersonaDefinition> {
                if (api.commonSources._cache.has(this._cacheKey)) {
                    return api.commonSources._cache.get(this._cacheKey);
                }

                const res = await fetch(this._base + "/definition.json");
                const data = await res.json();
                api.commonSources._cache.set(this._cacheKey, data.items);
                return data.items;
            },
            avatarByPath(avatar: string): string {
                return this._base + "/avatars/" + avatar;
            },
            async avatarByName(avatar: string): Promise<string> {
                const definition = await this.definition();
                return this.avatarByPath(definition[avatar].avatar);
            },
        },
        announcementTypes: {
            _base: "/common/announcements",
            _cacheKey: "announcement.types",
            async definition(): Promise<IAnnouncementTypesDefinition> {
                if (api.commonSources._cache.has(this._cacheKey)) {
                    return api.commonSources._cache.get(this._cacheKey);
                }

                const res = await fetch(this._base + "/types.json");
                const data = await res.json();
                api.commonSources._cache.set(this._cacheKey, data.items);
                return data.items;
            },
            async roleByType(announcementType: string): Promise<string> {
                const definition = await this.definition();
                return definition[announcementType].role;
            },
        },
    },
};
