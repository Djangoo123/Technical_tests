<script setup lang="ts">
import { computed } from "vue";
import MarkdownIt from "markdown-it";
import DOMPurify from "dompurify";

const props = defineProps<{ content?: string | null }>();

const md = new MarkdownIt({
  html: false,      // No html here
  linkify: true,
  breaks: true,
});

const html = computed(() => {
  const raw = props.content ?? "";
  const rendered = md.render(raw);

  return DOMPurify.sanitize(rendered, {
    USE_PROFILES: { html: true },
  });
});
</script>

<template>
  <div class="prose prose-slate max-w-none" v-html="html"></div>
</template>
