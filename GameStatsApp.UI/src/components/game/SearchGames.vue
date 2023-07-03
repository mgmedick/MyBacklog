<template>
    <div class="d-flex">
        <autocomplete v-model="searchText" @change="onChange" @search="onSearch" @selected="onSearchSelected" :options="searchResults" labelby="label" valueby="value" imageby="coverImagePath" :isasync="true" :isimgresults="true" :loading="searchLoading" :placeholder="'Search games'"/>    
    </div>  
</template>
<script>
    import axios from 'axios'

    export default {
        name: "SearchGames",
        data: function () {
            return {
                searchText: null,
                searchResults: [],
                searchLoading: false
            }
        },
        computed: {
        },
        watch: {
        },
        created: function () {
        },        
        methods: {
            onInput: function(e){
                this.searchText = e;
            },
            onChange: function() {
                var a = this.searchText;
            },
            onSearch: function() {
                var that = this;
                this.searchLoading = true;
                               
                axios.get('/Game/SearchGames', { params: { term: this.searchText } })
                        .then(res => {
                            that.searchResults = res.data;

                            if(that.searchResults.length == 0)
                            {
                                var noResult = { value: "", label: "No results found", disabled: true };
                                that.searchResults.push(noResult);
                            }

                            that.searchLoading = false;
                            return res;
                        })
                        .catch(err => { console.error(err); return Promise.reject(err); });
            },              
            onSearchSelected: function (result) {
                var controller;
                var action;

                console.log(result.value);
                // if (result.category == 'Users') {
                //     controller = "User";
                //     action = "UserDetails"
                // }
                
                // location.href = encodeURI('/' + controller + "/" + action + "/" + result.value);
            }
        }
    };
</script>





