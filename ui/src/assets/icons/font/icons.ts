export type IconsId =
  | "discord"
  | "edit"
  | "instagram"
  | "telegram";

export type IconsKey =
  | "Discord"
  | "Edit"
  | "Instagram"
  | "Telegram";

export enum Icons {
  Discord = "discord",
  Edit = "edit",
  Instagram = "instagram",
  Telegram = "telegram",
}

export const ICONS_CODEPOINTS: { [key in Icons]: string } = {
  [Icons.Discord]: "61697",
  [Icons.Edit]: "61698",
  [Icons.Instagram]: "61699",
  [Icons.Telegram]: "61700",
};
