export type IconsId =
  | "discord"
  | "edit"
  | "instagram"
  | "schedule"
  | "telegram"
  | "whatsapp";

export type IconsKey =
  | "Discord"
  | "Edit"
  | "Instagram"
  | "Schedule"
  | "Telegram"
  | "Whatsapp";

export enum Icons {
  Discord = "discord",
  Edit = "edit",
  Instagram = "instagram",
  Schedule = "schedule",
  Telegram = "telegram",
  Whatsapp = "whatsapp",
}

export const ICONS_CODEPOINTS: { [key in Icons]: string } = {
  [Icons.Discord]: "61697",
  [Icons.Edit]: "61698",
  [Icons.Instagram]: "61699",
  [Icons.Schedule]: "61700",
  [Icons.Telegram]: "61701",
  [Icons.Whatsapp]: "61702",
};
