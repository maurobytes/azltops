<template>
  <div class="list row">
    <div class="col-md-8">
      <div class="input-group mb-3">
        <input
          type="text"
          class="form-control"
          placeholder="Search by title"
          v-model="title"
        />
        <div class="input-group-append">
          <button
            class="btn btn-outline-secondary"
            type="button"
            @click="searchTitle"
          >
            Search
          </button>
        </div>
      </div>
    </div>
    <div class="col-md-6">
      <h4>Costumes List</h4>
      <ul class="list-group">
        <li
          class="list-group-item"
          :class="{ active: index == currentIndex }"
          v-for="(Costume, index) in Costumes"
          :key="index"
          @click="setActiveCostume(Costume, index)"
        >
          {{ Costume.title }}
        </li>
      </ul>
    </div>
    <div class="col-md-6">
      <div v-if="currentCostume.id">
        <h4>Costume</h4>
        <div>
          <label><strong>Title:</strong></label> {{ currentCostume.title }}
        </div>
        <div>
          <label><strong>Description:</strong></label>
          {{ currentCostume.description }}
        </div>
        <div>
          <label><strong>Spookyness level:</strong></label>
          {{ currentCostume.spookyness }}
        </div>

        <router-link
          :to="'/Costumes/' + currentCostume.id"
          class="badge-primary"
          >Edit</router-link
        >
      </div>
      <div v-else>
        <br />
        <p>Please click on a Costume...</p>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import CostumeDataService from "@/services/CostumeDataService";
import Costume from "@/types/Costume";
import ResponseData from "@/types/ResponseData";

export default defineComponent({
  name: "Costumes-list",
  data() {
    return {
      Costumes: [] as Costume[],
      currentCostume: {} as Costume,
      currentIndex: -1,
      title: "",
    };
  },
  methods: {
    retrieveCostumes() {
      CostumeDataService.getAll()
        .then((response: ResponseData) => {
          this.Costumes = response.data;
          console.log(response.data);
        })
        .catch((e: Error) => {
          console.log(e);
        });
    },

    refreshList() {
      this.retrieveCostumes();
      this.currentCostume = {} as Costume;
      this.currentIndex = -1;
    },

    setActiveCostume(Costume: Costume, index = -1) {
      this.currentCostume = Costume;
      this.currentIndex = index;
    },

    searchTitle() {
      CostumeDataService.findByTitle(this.title)
        .then((response: ResponseData) => {
          this.Costumes = response.data;
          this.setActiveCostume({} as Costume);
          console.log(response.data);
        })
        .catch((e: Error) => {
          console.log(e);
        });
    },
  },
  mounted() {
    this.retrieveCostumes();
  },
});
</script>

<style>
.list {
  text-align: left;
  max-width: 750px;
  margin: auto;
}
</style>
