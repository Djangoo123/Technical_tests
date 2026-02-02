<script setup lang="ts"></script>

<template>
  <div class="min-h-screen bg-gradient-to-b from-slate-50 to-white text-slate-900">
    <header class="sticky top-0 z-10 border-b bg-white/80 backdrop-blur">
      <div class="mx-auto max-w-6xl px-4 py-3 flex items-center justify-between">
        <router-link to="/" class="flex items-center gap-2 font-semibold tracking-tight">
          <span class="inline-flex h-8 w-8 items-center justify-center rounded-xl bg-slate-900 text-white text-sm">TT</span>
          <span>TechnicalTestCS</span>
        </router-link>

        <nav class="flex items-center gap-2 text-sm">
          <router-link
            to="/"
            class="rounded-xl px-3 py-2 hover:bg-slate-100"
            active-class="bg-slate-900 text-white hover:bg-slate-900"
            >Articles</router-link
          >
          <router-link
            to="/admin"
            class="rounded-xl px-3 py-2 hover:bg-slate-100"
            active-class="bg-slate-900 text-white hover:bg-slate-900"
            >Admin</router-link
          >
        </nav>
      </div>
    </header>

    <main class="mx-auto max-w-6xl px-4 py-6">
      <router-view />
    </main>

    <footer class="border-t bg-white">
      <div class="mx-auto max-w-6xl px-4 py-6 text-xs text-slate-500">
        Test technique • Vue + Tailwind • .NET
      </div>
    </footer>
  </div>
</template>
