<template>
  <div class="submit-form">
    <div v-if="!submitted">
      <div class="form-group">
        <label for="title">Title</label>
        <input
          type="text"
          class="form-control"
          id="title"
          required
          v-model="Costume.title"
          name="title"
        />
      </div>

      <div class="form-group">
        <label for="description">Description</label>
        <input
          class="form-control"
          id="description"
          required
          v-model="Costume.description"
          name="description"
        />
      </div>

      <div>
        <label for="slider">Spookyness Level</label>
        <Slider
          id="slider"
          v-model="Costume.spookyness"
          :min="1"
          :max="10"
          :step="1"
        />
      </div>

      <button @click="saveCostume" class="btn btn-success">Submit</button>
    </div>

    <div v-else>
      <h4>You submitted successfully!</h4>
      <button class="btn btn-success" @click="newCostume">Add</button>
    </div>
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
  name: "add-Costume",
  data() {
    return {
      Costume: {
        id: null,
        title: "",
        description: "",
        spookyness: 1,
      } as Costume,
      submitted: false,
    };
  },
  components: {
    Slider,
  },
  methods: {
    saveCostume() {
      let data = {
        title: this.Costume.title,
        description: this.Costume.description,
        spookyness: this.Costume.spookyness,
      };

      CostumeDataService.create(data)
        .then((response: ResponseData) => {
          this.Costume.id = response.data.id;
          console.log(response.data);
          this.submitted = true;
        })
        .catch((e: Error) => {
          console.log(e);
        });
    },

    newCostume() {
      this.submitted = false;
      this.Costume = {} as Costume;
    },
  },
});
</script>

<style>
.submit-form {
  max-width: 300px;
  margin: auto;
}

#slider {
  margin: 30px 0px;
}
</style>
