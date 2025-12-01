export type IconsId =
  | "discord"
  | "edit"
  | "instagram"
  | "schedule"
  | "whatsapp";

export type IconsKey =
  | "Discord"
  | "Edit"
  | "Instagram"
  | "Schedule"
  | "Whatsapp";

export enum Icons {
  Discord = "discord",
  Edit = "edit",
  Instagram = "instagram",
  Schedule = "schedule",
  Whatsapp = "whatsapp",
}

export const ICONS_CODEPOINTS: { [key in Icons]: string } = {
  [Icons.Discord]: "61697",
  [Icons.Edit]: "61698",
  [Icons.Instagram]: "61699",
  [Icons.Schedule]: "61700",
  [Icons.Whatsapp]: "61701",
};
