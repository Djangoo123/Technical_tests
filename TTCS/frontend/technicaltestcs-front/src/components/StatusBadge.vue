<script setup lang="ts">
import type { ArticleStatus } from "../types/ArticleStatus";

const props = defineProps<{ status: ArticleStatus }>();

const cls = (s: ArticleStatus) => {
  switch (s) {
    case "draft": return "bg-slate-100 text-slate-700 border-slate-200";
    case "pending": return "bg-amber-100 text-amber-800 border-amber-200";
    case "accepted": return "bg-green-100 text-green-800 border-green-200";
    case "rejected": return "bg-red-100 text-red-800 border-red-200";
  }
};
</script>

<template>
  <span class="text-xs border rounded-full px-2 py-1 font-medium" :class="cls(props.status)">
    {{ props.status }}
  </span>
</template>
