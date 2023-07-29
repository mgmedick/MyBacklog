<template>
    <div class="container games-container">
        <div class="row align-items-center mb-3">
            <div v-if="usergamelists.find(i => i.defaultGameListID == 1)?.id == usergamelistid" class="col-2 me-auto">
                <a href="/ImportGames" class="btn btn-secondary" tabindex="-1" role="button">
                    <font-awesome-icon icon="fa-solid fa-cloud-arrow-down" size="xl"/>
                </a>
            </div>           
            <div class="col-auto ms-auto d-flex">
                <button type="button" class="btn btn-secondary me-2" @click="onOrderByClick">
                    <font-awesome-icon v-if="orderByDesc" icon="fa-solid fa-caret-up" size="xl"/>
                    <font-awesome-icon v-else icon="fa-solid fa-caret-down" size="xl"/>
                </button>
                <input type="text" class="form-control" autocomplete="off" v-model="filterText" aria-describedby="spnUserNameErrors" placeholder="Filter games">         
            </div>
        </div>
        <div v-if="loading" class="center" style="font-size: 25px;">
            <font-awesome-icon icon="fa-solid fa-spinner" spin size="2xl"/>
        </div>
        <div v-else class="row g-3">
            <div class="col-lg-2 col-md-3 col-6">
                <div class="bg-light d-flex" style="height: 100%; cursor: pointer;" @click="onSearchGamesClick">
                    <font-awesome-icon icon="fa-solid fa-plus" size="2xl" class="mx-auto align-self-center" style="font-size: 50px; padding-top: 50%; padding-bottom: 50%;"/>
                </div>
            </div>
            <div v-for="(game, gameIndex) in games" class="col-lg-2 col-md-3 col-6 game-image-container" @mouseover="onGameImageMouseOver" @mouseleave="onGameImageMouseLeave" @click="onGameImageClick">
                <div class="delete-icon d-none" style="margin-bottom:-35px; margin-top: 8.5px; position: relative;">
                    <font-awesome-icon icon="fa-solid fa-circle-xmark" size="xl" class="d-flex ms-auto me-2" style="color: #d9534f; background: radial-gradient(#fff 50%, transparent 50%); cursor: pointer;" @click="onDeleteClick($event, game)"/>
                </div>
                <img :src="game.coverImagePath" class="img-fluid rounded" alt="Responsive image">
                <div class="gamelist-icons px-1 d-none" style="margin-top: -40px; margin-bottom: 10px;">
                    <div class="btn-group btn-group-sm gamelist-container" role="group" style="width: 100%;">
                        <button v-for="(userGameList, userGameListIndex) in usergamelists.filter(i => i.defaultGameListID && i.defaultGameListID != 1)" @click="onUserGameListClick($event, userGameList, game)" type="button" class="btn btn-light gamelist-item" :class="{ 'active' : game.userGameListIDs.indexOf(userGameList.id) > -1 }">
                            <font-awesome-icon :icon="getIconClass(userGameList.defaultGameListID)" size="lg"/>
                        </button>
                        <div v-if="usergamelists.filter(i => !i.defaultGameListID).length > 0" class="btn-group btn-group-sm gamelist-container" role="group">
                            <button type="button" class="btn btn-light dropdown-toggle" :class="{ 'active' : usergamelists.filter(i => !i.defaultGameListID && game.userGameListIDs.indexOf(i.id) > -1).length > 0 }" data-bs-toggle="dropdown" aria-expanded="false">
                                <font-awesome-icon icon="fa-solid fa-ellipsis" size="lg"/>
                            </button>
                            <ul class="dropdown-menu">
                                <li v-for="(userGameList, userGameListIndex) in usergamelists.filter(i => !i.defaultGameListID)"><a @click="onUserGameListClick($event, userGameList, game)" href="#" class="dropdown-item gamelist-item" :class="{ 'active' : game.userGameListIDs.indexOf(userGameList.id) > -1 }">{{ userGameList.name }}</a></li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>            
        </div>  
        <div ref="updatemodal" class="modal" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Games from linked accounts</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <p>Add latest games from your linked accounts?</p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-primary" @click="addGamesFromLinkedAccounts">Continue</button>
                </div>
                </div>
            </div>
        </div>          
        <div ref="removemodal" class="modal" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Remove Game</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <p>Remove <strong>{{ game?.name }}</strong> from all your lists?</p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-primary" @click="removeGameFromAllUserGameLists">Continue</button>
                </div>
                </div>
            </div>
        </div>  
        <div ref="searchmodal" class="modal modal-lg" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Add Game</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <autocomplete ref="searchAutocomplete" v-model="searchText" @search="onSearch" @selected="onSearchSelected" :options="searchResults" labelby="label" valueby="value" imageby="coverImagePath" :isasync="true" :isimgresults="true" :loading="searchLoading" :placeholder="'Search games'"/>    
                    </div>
                </div>
            </div>
        </div>                
    </div>
</template>
<script>
    import axios from 'axios';
    import { Modal } from 'bootstrap';

    export default {
        name: "UserGameListGames",
        props: {
            userid: String,
            usergamelistid: Number,
            usergamelists: Array
        },
        data: function () {
            return {
                games: [],
                allgames: [],
                game: {},
                loading: false,
                removeModal: {},
                searchModal: {},
                searchText: null,
                searchResults: [],
                searchLoading: false,
                filterText: null,
                orderByDesc: true   
            };
        },       
        watch: {
            usergamelistid: function (val, oldVal) {
                this.loadData();
            },
            filterText: function (val, oldVal) {
                var that = this;
                if (val != oldVal) {
                    that.filterResults(val);
                }
            }                        
        },  
        created: function () {
            this.loadData();
        },
        mounted: function() {
            var that = this;
            that.removeModal = new Modal(that.$refs.removemodal);
            that.searchModal = new Modal(that.$refs.searchmodal);
            that.$refs.searchmodal.addEventListener('hidden.bs.modal', event => {
                that.$refs.searchAutocomplete.clear();
            });
        },
        methods: {
            loadData: function () {
                var that = this;
                this.loading = true;

                axios.get('/User/GetGamesByUserGameList', { params: { userGameListID: this.usergamelistid } })
                    .then(res => {
                        that.games = res.data;
                        that.allgames = res.data.slice();
                        
                        if (that.orderByDesc) {
                            that.games = that.games.reverse();
                            that.allgames = that.allgames.reverse();
                        }

                        //that.loading = false;
                        return res;
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },
            getIconClass: function (id) {
                var iconClass = '';

                switch (id) {
                    case 1:
                        iconClass = 'fa-solid fa-list';
                        break;
                    case 2:
                        iconClass = 'fa-solid fa-inbox';
                        break;
                    case 3:
                        iconClass = 'fa-solid fa-play';
                        break;
                    case 4:
                        iconClass = 'fa-solid fa-check';
                        break;
                }

                return iconClass;
            },      
            onGameImageMouseOver(e) {
                var container = e.target.closest('.game-image-container');              
                container.querySelector('.gamelist-icons').classList.remove('d-none');
                container.querySelector('.delete-icon').classList.remove('d-none');
            },     
            onGameImageMouseLeave(e) {
                var container = e.target.closest('.game-image-container');
                if (!container.classList.contains('active')) {
                    container.querySelector('.gamelist-icons').classList.add('d-none');
                    container.querySelector('.delete-icon').classList.add('d-none');
                }
            }, 
            onGameImageClick(e) {
                var container = e.target.closest('.game-image-container');
                if (!container.classList.contains('active')) {
                    container.querySelector('.gamelist-icons').classList.remove('d-none');
                    container.querySelector('.delete-icon').classList.remove('d-none');
                    container.classList.add('active');
                } else {
                    container.querySelector('.gamelist-icons').classList.add('d-none');
                    container.querySelector('.delete-icon').classList.add('d-none');
                    container.classList.remove('active');                    
                }
            },  
            onDeleteClick(e, game) {
                var that = this;
                var el = e.target;
                
                if (!el.closest('.delete-icon').classList.contains('d-none')){
                    that.game = game;
                    that.removeModal.show();
                }
            },
            onUserGameListClick(e, userGameList, game) {
                var el = e.target;
                if (!el.closest('.gamelist-icons').classList.contains('d-none')) {
                    if (!el.closest('.gamelist-item').classList.contains('active')) {
                        this.addGameToUserGameList(userGameList, game, el);
                    } else {
                        this.removeGameFromUserGameList(userGameList, game, el);
                    }
                }
            },
            onSearchGamesClick(e){
                this.searchModal.show();
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
                var that = this;
                var usergamelist = this.usergamelists.find(item => item.id == that.usergamelistid);
                var userGameListIDs = [usergamelist.id];
                if (usergamelist.id != 1) {
                    userGameListIDs.unshift(1);
                }

                var game = { id: result.value, name: result.label, coverImagePath: result.coverImagePath, userGameListIDs: userGameListIDs };
                that.addGameToUserGameList(usergamelist, game).then(i => { that.searchModal.hide() });
            },  
            onFilterInput: function(e) {
                var val = e.target.value;
                if (val) {
                    this.filterResults(val);
                }
            },
            onOrderByClick(e){
                this.orderByDesc = !this.orderByDesc;
                this.games = this.games.reverse();
                this.allgames = this.allgames.reverse();                
            },
            filterResults(val) {
                var that = this;

                if (val) {
                    this.games = this.allgames.slice().filter((item) => {
                        return item.name.toLowerCase().indexOf(val.toLowerCase()) > -1;
                    });      
                } else {
                    this.games = this.allgames.slice();
                }                
            },  
            addGamesFromLinkedAccounts() {
                var that = this;

                return axios.post('/User/AddGamesFromUserLinkedAccounts')
                    .then((res) => {
                        if (res.data.success) {
                            that.loadData().then(i => { that.$emit('success', "Successfully added games from linked accounts") });                             
                        } else {
                            that.$emit('error', res.data.errorMessages);                           
                        }                        
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },                                
            addGameToUserGameList(userGameList, game, el) {
                var that = this;

                return axios.post('/User/AddGameToUserGameList', null,{ params: { userGameListID: userGameList.id, gameID: game.id } })
                    .then((res) => {
                        if (res.data.success) {
                            el?.closest('.gamelist-item').classList.add('active');
                            el?.closest('.gamelist-container').querySelector('button').classList.add('active');
                            if (that.games.filter(i => i.id == game.id).length == 0) {
                                that.games.push(game);
                            }                            
                            that.$emit('success', "Successfully added " + game.name + " to " + userGameList.name);
                        } else {
                            that.$emit('error', res.data.errorMessages);                           
                        }                        
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },          
            removeGameFromUserGameList(userGameList, game, el) {
                var that = this;
                
                return axios.post('/User/RemoveGameFromUserGameList', null,{ params: { userGameListID: userGameList.id, gameID: game.id } })
                    .then((res) => {
                        if (res.data.success) {
                            el.closest('.gamelist-item').classList.remove('active'); 
                            if (el.closest('.gamelist-container').querySelectorAll('.dropdown-item.active').length == 0){
                                el.closest('.gamelist-container').querySelector('button').classList.remove('active');                                                                                      
                            }                                                                              
                            that.$emit('success', "Successfully removed " + game.name + " from " + userGameList.name);
                        } else {
                            that.$emit('error', res.data.errorMessages);                           
                        }                        
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },
            removeGameFromAllUserGameLists() {
                var that = this;

                return axios.post('/User/RemoveGameFromAllUserGameLists', null,{ params: { gameID: that.game.id } })
                    .then((res) => {
                        if (res.data.success) {    
                            that.$emit('success', "Successfully removed " + that.game.name + " from all lists");
                            that.games = that.games.filter(i => i.id != that.game.id);            
                        } else {
                            that.$emit('error', res.data.errorMessages);                           
                        }   
                        that.removeModal.hide();  
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });                
            }                                        
        },
    };
</script>






