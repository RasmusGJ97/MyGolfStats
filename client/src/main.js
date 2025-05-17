import { createApp } from 'vue'
import '../src/assets/css/style.css'
import App from '../src/App.vue'
import router from './router'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'
import { createPinia } from 'pinia'
import '@vuepic/vue-datepicker/dist/main.css';


createApp(App).use(router).use(createPinia()).mount('#app')
