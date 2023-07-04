<template>
    <div class="dropdown">
        <div>
            <input type="text" class="form-control" :value="model" @input="model = $event.target.value" @click="onClick" @focus="onFocus" @keydown.down="onArrowDown" @keydown.up="onArrowUp" @keydown.enter="onEnter($event)" :placeholder="placeholder"/>
        </div>
        <div v-if="isimgresults" class="container">
            <div class="row g-3 mt-3">
                <div v-for="(result, i) in results" class="col-lg-2 col-md-3 col-4" @click="onSearchSelected(result)">
                    <img src="/dist/images/gamecovers/GameCover_yo1ypo1q.jpg" class="img-fluid rounded" alt="Responsive image">
                    <div class="text-xs">
                        <span>{{ result[labelby] }}</span>
                    </div>
                </div>
            </div>
        </div>
        <ul v-else class="dropdown-menu" style="width: 100%;" :style="[ isOpen ? { display:'block' } : { display:'none' } ]">
            <li v-for="(result, i) in results" :key="i" class="dropdown-item" :class="{ 'dropdown-header' : result.isGroupHeader, 'highlighted': i === arrowCounter }" @click="onSearchSelected(result)" @mouseover="arrowCounter = i">
                <span>{{ result[labelby] }}</span>
            </li>
        </ul>
  </div>    
</template>
<script>
    export default {
        name: "Autocomplete",
        emits: ["update:modelValue", "search", "selected"],
        props: {
            modelValue: String,
            options: {
                type: Array,
                default: () => []
            }, 
            labelby: {
                type: String,
                required: true
            },            
            valueby: {
                type: String,
                required: true
            },   
            imageby: {
                type: String,
                required: false
            },                    
            minlength: {
                type: Number,
                default: 0
            }, 
            isimgresults: Boolean,
            isasync: Boolean,               
            loading: Boolean,
            placeholder: String
        },
        data() {
            return {
                model: this.modelValue,
                results: [],
                isOpen: false,
                isFocus: false,
                arrowCounter: -1,
                throttleTimer: null,
                throttleDelay: 300                
            }
        },       
        watch: {
            options: function (val, oldVal) {
                this.results = val;
                this.isOpen = true;
            },       
            model: function (val, oldVal) {
                var that = this;
                this.$emit('update:modelValue', val); 

                this.$nextTick(function() {
                    if (val && val != oldVal) {
                        if (val.length >= that.minlength) {
                            if (that.isasync) {
                                clearTimeout(that.throttleTimer);
                                that.throttleTimer = setTimeout(function () {
                                    that.$emit('search'); 
                                }, that.throttleDelay);
                            } else {
                                that.filterResults();                            
                            }
                        }
                    }
                });
            }     
        },                    
        mounted() {
            if (!this.isasync) {
                this.results = this.options;
            }            
            document.addEventListener('click', this.handleClickOutside)
        },
        destroyed() {
            document.removeEventListener('click', this.handleClickOutside)
        },               
        methods: {    
            onClick() {
                if (!this.isasync) {
                    this.isOpen = true;
                }
            },             
            onFocus() {
                this.isFocus = !this.isFocus;
            },                  
            onArrowDown() {
                if (this.arrowCounter < this.results.length) {
                    this.arrowCounter = this.arrowCounter + 1;
                }
            },
            onArrowUp() {
                if (this.arrowCounter > 0) {
                    this.arrowCounter = this.arrowCounter - 1;
                }
            },
            onEnter(e) {
                e.preventDefault();
                var result = this.results[this.arrowCounter];
                if (result) {
                    this.onSearchSelected(result);
                }
            },    
            onSearchSelected: function (result) {   
                if (result.disabled){
                    return false;
                } else {
                    this.isOpen = false;
                    this.arrowCounter = -1;                     
                    this.model = result[this.valueby];                   
                    this.$emit('selected', result);
                }
            },          
            filterResults() {
                var that = this;

                this.results = this.options.filter((option) => {
                    return option[that.labelby].toLowerCase().indexOf(that.model.toLowerCase()) > -1;
                });

                if (this.results.length == 0) {
                    var noResult = { value: "", label: "No results found", category: null, disabled: true };
                    this.results.push(noResult);
                }                
            },                                    
            handleClickOutside(event) {
                if (!(this.$el == event.target || this.$el.contains(event.target))) {
                    this.isOpen = false;
                    this.isFocus = false;
                    this.arrowCounter = -1;
                }
            },
            clear() {
                this.model = "";
                this.results = [];
                // this.$el.querySelectorAll('.results').forEach(el => {
                //     el.innerHTML = "";
                // });
            }
        }
    };
</script>





