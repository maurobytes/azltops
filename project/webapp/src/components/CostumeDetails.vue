<template>
  <div v-if="currentCostume.id" class="edit-form">
    <h4>Costume</h4>
    <form>
      <div class="form-group">
        <label for="title">Title</label>
        <input
          type="text"
          class="form-control"
          id="title"
          v-model="currentCostume.title"
        />
      </div>
      <div class="form-group">
        <label for="description">Description</label>
        <input
          type="text"
          class="form-control"
          id="description"
          v-model="currentCostume.description"
        />
      </div>

      <div class="form-group">
        <label><strong>Spookyness:</strong></label>
        <Slider
          id="slider"
          v-model="currentCostume.spookyness"
          :min="1"
          :max="10"
          :step="1"
        />
      </div>
    </form>

    <button
      id="delete_btn"
      class="badge badge-danger mr-2"
      @click="deleteCostume"
    >
      Delete
    </button>

    <button
      id="update_btn"
      type="submit"
      class="badge badge-success"
      @click="updateCostume"
    >
      Update
    </button>
    <p>{{ message }}</p>
  </div>

  <div v-else>
    <br />
    <p>Please click on a Costume...</p>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import CostumeDataService from "@/services/CostumeDataService";
import Costume from "@/types/Costume";
import ResponseData from "@/types/ResponseData";
import Slider from "@vueform/slider";
import "@vueform/slider/themes/default.css";

export default defineComponent({
  name: "Costume",
  data() {
    return {
      currentCostume: {} as Costume,
      message: "",
    };
  },
  components: { Slider },
  methods: {
    getCostume(id: any) {
      CostumeDataService.get(id)
        .then((response: ResponseData) => {
          this.currentCostume = response.data;
          console.log(response.data);
        })
        .catch((e: Error) => {
          console.log(e);
        });
    },

    updateSpookyness(level: number) {
      let data = {
        id: this.currentCostume.id,
        title: this.currentCostume.title,
        description: this.currentCostume.description,
        spookyness: this.currentCostume.spookyness,
      };

      CostumeDataService.update(this.currentCostume.id, data)
        .then((response: ResponseData) => {
          console.log(response.data);
          this.currentCostume.spookyness = level;
          this.message = "The spookyness was updated successfully!";
        })
        .catch((e: Error) => {
          console.log(e);
        });
    },

    updateCostume() {
      CostumeDataService.update(this.currentCostume.id, this.currentCostume)
        .then((response: ResponseData) => {
          console.log(response.data);
          this.message = "The Costume was updated successfully!";
        })
        .catch((e: Error) => {
          console.log(e);
        });
    },

    deleteCostume() {
      CostumeDataService.delete(this.currentCostume.id)
        .then((response: ResponseData) => {
          console.log(response.data);
          this.$router.push({ name: "Costumes" });
        })
        .catch((e: Error) => {
          console.log(e);
        });
    },
  },
  mounted() {
    this.message = "";
    this.getCostume(this.$route.params.id);
  },
});
</script>

<style>
.edit-form {
  max-width: 300px;
  margin: auto;
}

#slider {
  margin: 30px 0px;
}

#delete_btn {
  margin-right: 20px;
  background-color: #ff6262;
}

#update_btn {
  background-color: #62ff62;
}
</style>
