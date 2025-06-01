<template>
  <div class="">
    <!-- Filter -->
    <input v-model="filterText" placeholder="Sök..." class="form-control mb-3 bg-secondary text-light border-0" style="opacity: 0.85;" />

    <!-- Tabell -->
    <table class="table table-dark table-bordered table-hover" style="opacity: 0.85;" >
      <thead>
        <tr>
          <th v-for="column in columns" :key="column.key" @click="sortBy(column.key)" class="cursor-pointer">
            {{ column.label }}
            <i
              v-if="sortKey === column.key"
              :class="[
                'ms-2',
                'fas',
                sortOrder === 'asc' ? 'fa-caret-up' : 'fa-caret-down'
              ]"
            ></i>
          </th>
          <th v-if="shouldShowTrashcanTH(columns)"></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="item in paginatedItems" :key="item.id" @click="$emit('row-clicked', item)" style="cursor: pointer;">
          <td v-for="column in columns" :key="column.key">
            <span v-if="Array.isArray(item[column.key])">
              {{ item[column.key].join(', ') }}
            </span>
            <span v-else-if="column.key === 'nettoScore' && item.par">
              <span :class="{'text-success': item.nettoScore < item.par}">
                {{ item[column.key] }}
              </span>
            </span>
            <span v-else>
              {{ item[column.key] }}
            </span>
          </td>
          <td v-if="shouldShowTrashcan(item)" class="text-center align-middle">
            <i
              class="fas fa-trash-alt text-danger"
              @click.stop="$emit('delete-item', item)"
              style="cursor: pointer;"
            ></i>
          </td>
        </tr>
        <tr v-if="paginatedItems.length === 0">
          <td :colspan="columns.length" class="text-center">Inga resultat</td>
        </tr>
      </tbody>
    </table>

    <!-- Paging -->
    <div class="d-flex justify-content-between align-items-center mt-3">
      <div>
        Visar {{ startItem + 1 }} - {{ endItem }} av {{ filteredItems.length }}
      </div>
      <nav>
        <ul class="pagination pagination-sm mb-0">
          <li class="page-item" :class="{ disabled: currentPage === 1 }">
            <button class="page-link bg-dark text-light border-0" @click="goToPage(1)">Första</button>
          </li>
          <li class="page-item" :class="{ disabled: currentPage === 1 }">
            <button class="page-link bg-dark text-light border-0" @click="goToPage(currentPage - 1)">«</button>
          </li>
          <li v-for="page in visiblePageNumbers" :key="page" class="page-item" :class="{ active: page === currentPage }">
            <button class="page-link" :class="page === currentPage ? 'bg-secondary text-light border-0' : 'bg-dark text-light border-0'" @click="goToPage(page)">
              {{ page }}
            </button>
          </li>
          <li class="page-item" :class="{ disabled: currentPage === totalPages }">
            <button class="page-link bg-dark text-light border-0" @click="goToPage(currentPage + 1)">»</button>
          </li>
          <li class="page-item" :class="{ disabled: currentPage === totalPages }">
            <button class="page-link bg-dark text-light border-0" @click="goToPage(totalPages)">Sista</button>
          </li>
        </ul>
      </nav>
    </div>
  </div>
</template>

<script>
import { useUserStore } from '../stores/userStore';

export default {
  name: 'DataGrid',
  props: {
    items: {
      type: Array,
      required: true
    },
    columns: {
      type: Array,
      required: true
    },
    pageSize: {
      type: Number,
      default: 10
    },
    maxVisiblePages: {
      type: Number,
      default: 5
    }
  },
  data() {
    return {
      filterText: '',
      sortKey: '',
      sortOrder: 'asc',
      currentPage: 1
    };
  },
  computed: {
    userStore() {
      return useUserStore()
    },
    currentUser() {
      return this.userStore.user
    },
    filteredItems() {
      if (!this.filterText) return this.items;
      const text = this.filterText.toLowerCase();
      return this.items.filter(item =>
        Object.values(item).some(value =>
          String(value).toLowerCase().includes(text)
        )
      );
    },
    sortedItems() {
      if (!this.sortKey) return this.filteredItems;
      return [...this.filteredItems].sort((a, b) => {
        const valA = a[this.sortKey];
        const valB = b[this.sortKey];
        if (valA < valB) return this.sortOrder === 'asc' ? -1 : 1;
        if (valA > valB) return this.sortOrder === 'asc' ? 1 : -1;
        return 0;
      });
    },
    paginatedItems() {
      const start = (this.currentPage - 1) * this.pageSize;
      return this.sortedItems.slice(start, start + this.pageSize);
    },
    totalPages() {
      return Math.max(1, Math.ceil(this.filteredItems.length / this.pageSize));
    },
    startItem() {
      return (this.currentPage - 1) * this.pageSize;
    },
    endItem() {
      return Math.min(this.startItem + this.paginatedItems.length, this.filteredItems.length);
    },
    visiblePageNumbers() {
      const half = Math.floor(this.maxVisiblePages / 2);
      let start = Math.max(1, this.currentPage - half);
      let end = Math.min(this.totalPages, start + this.maxVisiblePages - 1);
      if (end - start < this.maxVisiblePages - 1) {
        start = Math.max(1, end - this.maxVisiblePages + 1);
      }
      const pages = [];
      for (let i = start; i <= end; i++) {
        pages.push(i);
      }
      return pages;
    }
  },
  watch: {
    filterText() {
      this.currentPage = 1;
    },
    sortKey() {
      this.currentPage = 1;
    }
  },
  methods: {
    shouldShowTrashcan(item) {
      const role = this.currentUser.role;
      if (!role) return false;

      if ('holeCount' in item && role === 'Admin') {
        return true;
      }

      if ('bruttoScore' in item && (role === 'User' || role === 'Admin') ) {
        return true;
      }

      return false;
    },
    shouldShowTrashcanTH(columns) {
      const role = this.currentUser?.role;
      if (!role) return false;

      return columns.some(column =>
        (column.key === 'holeCount' && role === 'Admin') ||
        (column.key === 'bruttoScore' && (role === 'User' || role === 'Admin'))
      );
    },
    sortBy(key) {
      if (this.sortKey === key) {
        this.sortOrder = this.sortOrder === 'asc' ? 'desc' : 'asc';
      } else {
        this.sortKey = key;
        this.sortOrder = 'asc';
      }
    },
    goToPage(page) {
      if (page >= 1 && page <= this.totalPages) {
        this.currentPage = page;
      }
    }
  }
};
</script>

<style scoped>
.cursor-pointer {
  cursor: pointer;
}
.table td, .table th {
  vertical-align: middle;
}
</style>
