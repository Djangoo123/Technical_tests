<script setup lang="ts">
import { onMounted, ref, computed } from "vue";
import { getAdminArticles, updateArticleStatus } from "../api/articles";
import type { ArticleSummaryDto } from "../types/ArticleSummaryDto";
import type { ArticleStatus } from "../types/ArticleStatus";
import StatusBadge from "../components/StatusBadge.vue";
import { nextActions } from "../utils/Actions";

const loading = ref(true);
const error = ref<string | null>(null);
const items = ref<ArticleSummaryDto[]>([]);

const q = ref("");
const status = ref<ArticleStatus | "">("");

const params = computed(() => ({
  q: q.value.trim() || undefined,
  status: (status.value || undefined) as ArticleStatus | undefined,
}));


async function load() {
  loading.value = true;
  error.value = null;
  try {
    items.value = await getAdminArticles(params.value);
  } catch (e: any) {
    error.value = e?.message ?? "Erreur";
  } finally {
    loading.value = false;
  }
}

async function changeStatus(id: number, to: ArticleStatus) {
  error.value = null;
  try {
    const res = await updateArticleStatus(id, to);
    // update local list
    items.value = items.value.map((x) => (x.id === id ? { ...x, status: res.status } : x));
  } catch (e: any) {
    error.value = e?.message ?? "Erreur";
  }
}

onMounted(load);
</script>

<template>
  <div class="space-y-4">
    <div class="flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
      <div>
        <h1 class="text-xl font-semibold">Admin</h1>
        <p class="text-sm text-slate-600">Changer les statuts selon la machine à états.</p>
      </div>

      <div class="flex gap-2">
        <input v-model="q" class="w-full sm:w-64 rounded-xl border px-3 py-2 text-sm" placeholder="Rechercher…" @keyup.enter="load" />
        <select v-model="status" class="rounded-xl border px-3 py-2 text-sm">
          <option value="">Tous</option>
          <option value="draft">draft</option>
          <option value="pending">pending</option>
          <option value="accepted">accepted</option>
          <option value="rejected">rejected</option>
        </select>
        <button class="rounded-xl bg-slate-900 px-4 py-2 text-sm text-white" @click="load">Filtrer</button>
      </div>
    </div>

    <div v-if="error" class="rounded-xl border border-red-200 bg-red-50 p-3 text-sm text-red-800 whitespace-pre-wrap">
      {{ error }}
    </div>

    <div v-if="loading" class="text-sm text-slate-600">Chargement…</div>

    <div v-else class="space-y-3">
      <div
        v-for="a in items"
        :key="a.id"
        class="rounded-2xl border bg-white p-4 shadow-sm flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between"
      >
        <div class="min-w-0">
          <div class="flex items-center gap-2">
            <div class="font-semibold truncate">{{ a.title }}</div>
            <StatusBadge :status="a.status" />
          </div>
          <div class="text-xs text-slate-500">ID: {{ a.id }}</div>
        </div>

      <div class="flex flex-wrap gap-2">
        <button
          v-for="act in nextActions(a.status)"
          :key="act.label"
          :disabled="('disabled' in act) ? act.disabled : false"
          class="rounded-xl border px-3 py-2 text-xs disabled:opacity-50 disabled:cursor-not-allowed hover:bg-slate-50"
          @click="!('disabled' in act) && changeStatus(a.id, act.status)"
        >
          {{ act.label }}
        </button>
      </div>

      </div>
    </div>
  </div>
</template>
