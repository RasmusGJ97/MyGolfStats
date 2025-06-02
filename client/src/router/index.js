import { createRouter, createWebHistory } from 'vue-router'
import Home from '../pages/Home.vue'
import Login from '../pages/Login.vue'
import Courses from '../pages/Courses.vue'
import Rounds from '../pages/Rounds.vue'
import Settings from '../pages/Settings.vue'
import Statistics from '../pages/Statistics.vue'
import ForgotPassword from '../pages/ForgotPassword.vue'
import ResetPassword from '../pages/ResetPassword.vue'

import { isLoggedIn } from '../api/MyGolfStatsApi'

const routes = [
  { path: '/', component: Home, meta: {requiresAuth: true } },
  { path: '/login', component: Login },
  { path: '/forgot-password', component: ForgotPassword },
  { path: '/reset-password', component: ResetPassword },
  { path: '/courses', component: Courses, meta: {requiresAuth: true } },
  { path: '/rounds', component: Rounds, meta: {requiresAuth: true } },
  { path: '/settings', component: Settings, meta: {requiresAuth: true } },
  { path: '/statistics', component: Statistics, meta: {requiresAuth: true } },
  { path: "/course/:id", name: "CourseForm", component: () => import("../pages/CourseForm.vue"), meta: {requiresAuth: true } },
  { path: "/round/:id", name: "RoundForm", component: () => import("../pages/RoundForm.vue"), meta: {requiresAuth: true } },
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  if (to.meta.requiresAuth && !isLoggedIn()) {
    next('/login');
  } else if (to.path === '/login' && isLoggedIn()) {
    next('/');
  } else {
    next();
  }
});

export default router