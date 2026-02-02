<script setup lang="ts">
import { onMounted, ref, computed } from "vue";
import { useRoute } from "vue-router";
import StatusBadge from "../components/StatusBadge.vue";
import MarkdownContent from "../components/MarkdownContent.vue";
import { getArticle } from "../api/articles";
import type { ArticleDetailDto } from "../types/ArticleDetailDto";

const route = useRoute();
const loading = ref(true);
const error = ref<string | null>(null);
const item = ref<ArticleDetailDto | null>(null);

const heroSrc = computed<string | undefined>(() => {
  const src = item.value?.bannerUrl ?? item.value?.imageUrl;
  return src ?? undefined; 
});

async function load() {
  loading.value = true;
  error.value = null;

  try {
    const id = Number(route.params.id);
    item.value = await getArticle(id);
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
    <div
      v-if="error"
      class="rounded-2xl border border-red-200 bg-red-50 p-4 text-sm text-red-800 whitespace-pre-wrap"
    >
      {{ error }}
    </div>

    <div v-if="loading" class="text-sm text-slate-600">Chargement…</div>

    <div v-else-if="item" class="space-y-6">
      <!-- Main card -->
      <div class="rounded-2xl border bg-white overflow-hidden shadow-sm">
<div class="relative aspect-[21/9] bg-slate-100">
  <img v-if="heroSrc" :src="heroSrc" class="h-full w-full object-contain bg-slate-100" />
          <div
            v-else
            class="h-full w-full flex items-center justify-center text-slate-400 text-sm"
          >
            Pas d’image
          </div>

          <div class="absolute top-4 right-4">
            <StatusBadge :status="item.status" />
          </div>
        </div>

        <div class="p-5 sm:p-7 space-y-4">
          <div class="space-y-1">
            <h1 class="text-2xl font-semibold tracking-tight leading-snug">
              {{ item.title }}
            </h1>
            <p v-if="item.slug" class="text-sm text-slate-500">{{ item.slug }}</p>
          </div>

          <MarkdownContent v-if="item.content" :content="item.content" />
          <div v-else class="text-sm text-slate-600">Pas de contenu.</div>
        </div>
      </div>

      <!-- Partners -->
      <section v-if="item.partners?.length" class="space-y-3">
        <div class="flex items-end justify-between">
          <h2 class="text-lg font-semibold">Partenaires</h2>
          <span class="text-xs text-slate-500">{{ item.partners.length }}</span>
        </div>

        <div class="grid gap-3 sm:grid-cols-2">
          <div
            v-for="p in item.partners"
            :key="p.id"
            class="rounded-2xl border bg-white p-4 shadow-sm"
          >
            <div class="flex gap-3">
              <div
                class="h-12 w-12 rounded-xl bg-slate-100 overflow-hidden flex items-center justify-center shrink-0"
              >
                <img
                  v-if="p.logoUrl"
                  :src="p.logoUrl ?? undefined"
                  class="h-full w-full object-cover"
                  loading="lazy"
                />
                <span v-else class="text-slate-400 text-xs">Logo</span>
              </div>

              <div class="min-w-0">
                <div class="font-medium truncate">{{ p.name }}</div>
                <a
                  v-if="p.website"
                  :href="p.website"
                  target="_blank"
                  rel="noreferrer"
                  class="text-xs text-slate-600 hover:underline truncate block"
                >
                  {{ p.website }}
                </a>
              </div>
            </div>

            <p v-if="p.description" class="mt-3 text-sm text-slate-700 line-clamp-4">
             <MarkdownContent v-if="item.content" :content="p.description" />
            </p>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>
