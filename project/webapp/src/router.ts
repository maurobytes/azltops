import { createWebHistory, createRouter, RouteRecordRaw } from "vue-router";

const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    alias: "/costumes",
    name: "Costumes",
    component: () => import("./components/CostumesList.vue"),
  },
  {
    path: "/costumes/:id",
    name: "costume-details",
    component: () => import("./components/CostumeDetails.vue"),
  },
  {
    path: "/add",
    name: "add",
    component: () => import("./components/AddCostume.vue"),
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
