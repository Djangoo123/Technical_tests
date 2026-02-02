<script setup lang="ts">
import { onMounted, ref, computed } from "vue";
import StatusBadge from "../components/StatusBadge.vue";
import type { ArticleSummaryDto } from "../types/ArticleSummaryDto";
import type { ArticleStatus } from "../types/ArticleStatus";
import { getArticles } from "../api/articles";

const loading = ref(true);
const error = ref<string | null>(null);
const items = ref<ArticleSummaryDto[]>([]);

const q = ref("");
const status = ref<ArticleStatus | "">("");

const filteredParams = computed(() => ({
  q: q.value.trim() || undefined,
  status: (status.value || undefined) as ArticleStatus | undefined,
}));

async function load() {
  loading.value = true;
  error.value = null;
  try {
    items.value = await getArticles(filteredParams.value);
  } catch (e: any) {
    error.value = e?.message ?? "Erreur";
  } finally {
    loading.value = false;
  }
}

onMounted(load);
</script>

<template>
  <div class="space-y-6">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
      <div>
        <h1 class="text-2xl font-semibold tracking-tight">Articles</h1>
      </div>

      <div class="w-full sm:w-auto flex flex-col gap-2 sm:flex-row sm:items-center">
        <div class="flex gap-2">
          <input
            v-model="q"
            class="w-full sm:w-72 rounded-xl border bg-white px-3 py-2 text-sm shadow-sm focus:outline-none focus:ring-2 focus:ring-slate-300"
            placeholder="Rechercher (titres…)"
            @keyup.enter="load"
          />
          <button
            class="shrink-0 rounded-xl bg-slate-900 px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-slate-800"
            @click="load"
          >
            Rechercher
          </button>
        </div>

        <div class="flex gap-2">
          <select
            v-model="status"
            class="w-full rounded-xl border bg-white px-3 py-2 text-sm shadow-sm focus:outline-none focus:ring-2 focus:ring-slate-300"
          >
            <option value="">Tous statuts</option>
            <option value="draft">draft</option>
            <option value="pending">pending</option>
            <option value="accepted">accepted</option>
            <option value="rejected">rejected</option>
          </select>

          <button
            class="shrink-0 rounded-xl border bg-white px-4 py-2 text-sm shadow-sm hover:bg-slate-50"
            @click="
              q = '';
              status = '';
              load();
            "
          >
            Reset
          </button>
        </div>
      </div>
    </div>

    <div
      v-if="error"
      class="rounded-2xl border border-red-200 bg-red-50 p-4 text-sm text-red-800"
    >
      {{ error }}
    </div>

    <div v-if="loading" class="text-sm text-slate-600">Chargement…</div>

    <div v-else class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      <router-link
        v-for="a in items"
        :key="a.id"
        :to="`/articles/${a.id}`"
        class="group rounded-2xl border bg-white shadow-sm hover:shadow-md transition overflow-hidden"
      >
        <div class="relative aspect-[16/10] bg-slate-100 overflow-hidden">
          <img
            v-if="a.imageUrl"
            :src="a.imageUrl"
            class="h-full w-full object-cover group-hover:scale-[1.03] transition"
            loading="lazy"
          />
          <div v-else class="h-full w-full flex items-center justify-center text-slate-400 text-sm">
            Pas d’image
          </div>

          <div class="absolute top-3 right-3">
            <StatusBadge :status="a.status" />
          </div>
        </div>

        <div class="p-4 space-y-2">
          <h2 class="font-semibold leading-snug line-clamp-2">
            {{ a.title }}
          </h2>
          <div class="flex items-center justify-between text-xs text-slate-500">
            <span>ID: {{ a.id }}</span>
            <span v-if="a.slug" class="truncate max-w-[12rem]">{{ a.slug }}</span>
          </div>
        </div>
      </router-link>
    </div>
  </div>
</template>

