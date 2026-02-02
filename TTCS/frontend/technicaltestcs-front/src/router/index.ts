import { createRouter, createWebHistory } from "vue-router";
import ArticlesListView from "../views/ArticlesListView.vue";
import ArticleDetailView from "../views/ArticleDetailView.vue";
import AdminView from "../views/AdminView.vue";

export const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: "/", name: "home", component: ArticlesListView },
    { path: "/articles/:id", name: "article", component: ArticleDetailView, props: true },
    { path: "/admin", name: "admin", component: AdminView },
  ],
});
