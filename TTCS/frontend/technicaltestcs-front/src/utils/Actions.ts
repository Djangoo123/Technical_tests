import type { ArticleStatus } from "../types/ArticleStatus";

export type Action =
    | { label: string; status: ArticleStatus }
    | { label: string; disabled: true };

const ACTIONS = {
draft: [{ label: "Passer en pending", status: "pending" }],
pending: [
{ label: "Accepter", status: "accepted" },
{ label: "Rejeter", status: "rejected" },
],
rejected: [{ label: "Repasser en pending", status: "pending" }],
accepted: [{ label: "Aucune action", disabled: true }],
} satisfies Record<ArticleStatus, Action[]>;

export function nextActions(current: ArticleStatus): Action[] {
  return ACTIONS[current];
}