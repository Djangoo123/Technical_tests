<script setup lang="ts">
import { computed } from "vue";
import MarkdownIt from "markdown-it";
import DOMPurify from "dompurify";

const props = defineProps<{ content?: string | null }>();

/**
 * Markdown renderer:
 * - html:true to allow backend-provided HTML blocks
 * - still sanitized by DOMPurify before rendering
 */
const md = new MarkdownIt({
  html: true,
  linkify: true,
  breaks: true,
});

// Note : can have strange effects
// Todo : make a module sanitize
DOMPurify.addHook("afterSanitizeAttributes", (node) => {
  if (node instanceof HTMLAnchorElement) {
    const href = (node.getAttribute("href") || "").trim().toLowerCase();
    if (href.startsWith("javascript:")) node.removeAttribute("href");

    node.setAttribute("rel", "noopener noreferrer");
    node.setAttribute("target", "_blank");
  }
});

const html = computed(() => {
  const raw = props.content ?? "";
  const rendered = md.render(raw);

  return DOMPurify.sanitize(rendered, {
    USE_PROFILES: { html: true },

    // in case of attacks
    FORBID_TAGS: ["script", "style", "iframe", "object", "embed", "svg", "math"],
    FORBID_ATTR: ["style"],
  });
});
</script>

<template>
  <div class="prose prose-slate max-w-none" v-html="html"></div>
</template>
