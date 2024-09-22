export type IconsId =
  | "discord"
  | "edit"
  | "instagram"
  | "schedule"
  | "telegram";

export type IconsKey =
  | "Discord"
  | "Edit"
  | "Instagram"
  | "Schedule"
  | "Telegram";

export enum Icons {
  Discord = "discord",
  Edit = "edit",
  Instagram = "instagram",
  Schedule = "schedule",
  Telegram = "telegram",
}

export const ICONS_CODEPOINTS: { [key in Icons]: string } = {
  [Icons.Discord]: "61697",
  [Icons.Edit]: "61698",
  [Icons.Instagram]: "61699",
  [Icons.Schedule]: "61700",
  [Icons.Telegram]: "61701",
};
