<template>
    <div class="container games-container p-0">
        <div class="d-flex align-items-center mb-3">
            <div v-if="userlists.find(i => i.defaultListID == 1).id == userlistid" class="me-2">
                <a v-if="selectedUserAccountID" href="/ImportGames" class="btn btn-secondary ps-2 pe-3" tabindex="-1" role="button">
                    <font-awesome-layers class="fa-2xl">
                        <font-awesome-icon icon="fa-solid fa-spinner m-2" spin transform="shrink-5" style="color: #adb5bd; margin-left: 0.25rem; z-index: 9999;"/>
                        <font-awesome-icon icon="fa-solid fa-cloud"/>
                    </font-awesome-layers>
                </a>
                <a v-else href="/ImportGames" class="btn btn-secondary p-2" tabindex="-1" role="button">
                    <font-awesome-icon icon="fa-solid fa-cloud-arrow-down" size="2xl"/>
                </a>                
            </div>        
            <div class="d-flex ms-auto">   
                <div class="btn-group me-1" role="group">
                    <button type="button" class="btn btn-secondary p-2" @click="onOrderByDescClick"><font-awesome-icon v-if="orderByDesc" icon="fa-solid fa-arrow-down-wide-short" size="xl"/><font-awesome-icon v-else icon="fa-solid fa-arrow-up-wide-short" size="xl"/></button>
                    <button type="button" class="btn btn-secondary dropdown-toggle dropdown-toggle-split p-2" data-bs-toggle="dropdown" aria-expanded="false">
                        <span class="visually-hidden">Toggle Dropdown</span>
                    </button>                    
                    <ul class="dropdown-menu">
                        <li><a href="#" @click="onOrderByOptionClick($event, 0)" class="dropdown-item" :class="{ 'active' : orderByID == 0 }">Date Added</a></li>
                        <li><a href="#" @click="onOrderByOptionClick($event, 1)" class="dropdown-item" :class="{ 'active' : orderByID == 1 }">Name</a></li>
                    </ul>
                </div>                             
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
            <div v-for="(game, gameIndex) in games" class="col-lg-2 col-md-3 col-6 game-image-container" :key="game.id" @mouseover="onGameImageMouseOver" @mouseleave="onGameImageMouseLeave" @click="onGameImageClick">
                <div class="delete-icon d-none" style="margin-bottom:-35px; margin-top: 8.5px; position: relative;">
                    <font-awesome-icon icon="fa-solid fa-circle-xmark" size="xl" class="d-flex ms-auto me-2" style="color: #d9534f; background: radial-gradient(#fff 50%, transparent 50%); cursor: pointer;" @click="onDeleteClick($event, game)"/>
                </div>                
                <img v-if="game.coverImagePath" :src="game.coverImagePath" class="img-fluid rounded" alt="Responsive image">
                <div v-else class="bg-dark d-flex align-items-center" style="height: 100%; text-align: center; padding-top: 50%; padding-bottom: 50%;">
                    <span style="color: #fff; width:100%; min-height: 50px;">{{ game.name }}</span>
                </div>
                <div class="gamelist-icons px-1 d-none" style="margin-top: -50px; margin-bottom: 10px;">
                    <div class="btn-group" role="group" style="width: 100%;">
                        <button v-for="(userList, userListIndex) in userlists.filter(i => i.defaultListID && i.defaultListID != 1)" :key="userList.id" @click="onUserListClick($event, userList, game)" type="button" class="btn btn-light gamelist-item" :class="{ 'active' : game.userListIDs.indexOf(userList.id) > -1 }" :data-val="userList.id">
                            <font-awesome-icon :icon="getIconClass(userList.defaultListID)" size="lg"/>
                        </button>
                        <div v-if="userlists.filter(i => !i.defaultListID).length > 0" class="btn-group btn-group-sm gamelist-btn-group" role="group">
                            <button type="button" class="btn btn-light dropdown-toggle" :class="{ 'active' : userlists.filter(i => !i.defaultListID && game.userListIDs.indexOf(i.id) > -1).length > 0 }" data-bs-toggle="dropdown" aria-expanded="false">
                                <font-awesome-icon icon="fa-solid fa-ellipsis" size="lg"/>
                            </button>
                            <ul class="dropdown-menu">
                                <li v-for="(userList, userListIndex) in userlists.filter(i => !i.defaultListID)" :key="userList.id"><a @click="onUserListClick($event, userList, game)" href="#" class="dropdown-item gamelist-item" :class="{ 'active' : game.userListIDs.indexOf(userList.id) > -1 }">{{ userList.name }}</a></li>
                            </ul>
                        </div>
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
                        <p>Remove <strong>{{ game.name }}</strong> from all your lists?</p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        <button type="button" class="btn btn-primary" @click="onRemoveGameClick($event, game)">Continue</button>
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
    import { successToast, errorToast } from '../../js/common.js';

    export default {
        name: "UserListGames",
        props: {
            userid: String,
            userlistid: Number,
            userlists: Array
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
                orderByDesc: true,             
                orderByID: 0
            };
        },       
        watch: {
            userlistid: function (val, oldVal) {
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

                axios.get('/User/GetUserListGames', { params: { userListID: this.userlistid } })
                    .then(res => {
                        that.games = res.data;
                        that.allgames = res.data.slice();                      
                        that.sortGames();

                        that.loading = false;
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
                var gamelisticons = e.target.closest('.gamelist-icons');
                if (!container.classList.contains('active')) {
                    container.querySelector('.gamelist-icons').classList.remove('d-none');
                    container.querySelector('.delete-icon').classList.remove('d-none');
                    container.classList.add('active');
                } else if (!gamelisticons) {
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
            onRemoveGameClick(e, game) {
                this.removeGameFromAllUserLists(game);
            },            
            onUserListClick(e, userList, game) {
                var el = e.target;
                if (!el.closest('.gamelist-icons').classList.contains('d-none')) {
                    if (!el.closest('.gamelist-item').classList.contains('active')) {
                        this.addGameToUserList(userList, game, el);
                    } else {
                        this.removeGameFromUserList(userList, game, el);
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
                var userlist = this.userlists.find(item => item.id == that.userlistid);
                var userListIDs = [userlist.id];
                if (userlist.id != 1) {
                    userListIDs.unshift(1);
                }

                var game = { id: result.value, name: result.label, coverImagePath: result.coverImagePath, userListIDs: userListIDs };
                that.addGameToUserList(userlist, game).then(i => { that.searchModal.hide() });
            },  
            onFilterInput: function(e) {
                var val = e.target.value;
                if (val) {
                    this.filterResults(val);
                }
            },
            onOrderByDescClick(e){
                this.orderByDesc = !this.orderByDesc;
                this.sortGames();                
            },            
            onOrderByOptionClick(e, val){
                this.orderByID = val;
                this.sortGames();                
            },
            sortGames() {
                switch (this.orderByID)
                {
                    case 0:
                        if (this.orderByDesc) {
                            this.games = this.games.sort((a, b) => { return b.id - a.id });
                            this.allgames = this.allgames.sort((a, b) => { return b.id - a.id });
                        } else {
                            this.games = this.games.sort((a, b) => { return a.id - b.id });
                            this.allgames = this.allgames.sort((a, b) => { return a.id - b.id });
                        }
                        break;                    
                    case 1:
                        if (this.orderByDesc) {
                            this.games = this.games.sort((a, b) => { return b.name.localeCompare(a.name) });
                            this.allgames = this.allgames.sort((a, b) => { return b.name.localeCompare(a.name) });
                        } else {
                            this.games = this.games.sort((a, b) => { return a.name.localeCompare(b.name) });
                            this.allgames = this.allgames.sort((a, b) => { return a.name.localeCompare(b.name) });
                        }
                        break;                                                                              
                }
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
            addGameToUserList(userList, game, el) {
                var that = this;
                el.closest('.gamelist-item').classList.add('active');
                el.closest('.gamelist-btn-group button')?.classList.add('active');

                return axios.post('/User/AddGameToUserList', null,{ params: { userListID: userList.id, gameID: game.id } })
                    .then((res) => {
                        if (res.data.success) {
                            if (game.userListIDs.indexOf(userList.id) == -1) {
                                game.userListIDs.push(userList.id);
                            } 

                            if (that.games.filter(i => i.id == game.id).length == 0) {
                                that.games.push(game);
                                that.allgames.push({...game});
                            }
                            successToast("Added <strong>" + game.name + "</strong> to <strong>" + userList.name + "</strong>");
                        } else {                        
                            if (game.userListIDs.indexOf(userList.id) > -1) {
                                game.userListIDs.splice(game.userListIDs.indexOf(userList.id),1);
                            } 

                            res.data.errorMessages.forEach(errorMsg => {
                                errorToast(errorMsg);                           
                            });                              
                        }                        
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },          
            removeGameFromUserList(userList, game, el) {
                var that = this;                
                el.closest('.gamelist-item').classList.remove('active');
                el.closest('.gamelist-btn-group button')?.classList.add('active');

                return axios.post('/User/RemoveGameFromUserList', null,{ params: { userListID: userList.id, gameID: game.id } })
                    .then((res) => {
                        if (res.data.success) {
                            game.userListIDs = game.userListIDs.filter(i => i != userList.id);

                            if (that.userlistid == userList.id) {
                                that.games = that.games.filter(i => i.id != game.id);            
                                that.allgames = that.allgames.filter(i => i.id != game.id);  
                            }
                            successToast("Removed <strong>" + game.name + "</strong> from <strong>" + userList.name + "</strong>");
                        } else {
                            if (game.userListIDs.indexOf(userList.id) == -1) {
                                game.userListIDs.push(userList.id);
                            } 

                            res.data.errorMessages.forEach(errorMsg => {
                                errorToast(errorMsg);                           
                            });                           
                        }                        
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },
            removeGameFromAllUserLists(game) {
                var that = this;

                return axios.post('/User/RemoveGameFromAllUserLists', null,{ params: { gameID: game.id } })
                    .then((res) => {
                        if (res.data.success) {    
                            that.games = that.games.filter(i => i.id != game.id);            
                            that.allgames = that.allgames.filter(i => i.id != game.id);            
                            successToast("Removed <strong>" + game.name + "</strong> from <strong>all lists</strong>");
                        } else {
                            res.data.errorMessages.forEach(errorMsg => {
                                errorToast(errorMsg);                           
                            });                           
                        }   
                        that.removeModal.hide();  
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });                
            }                                                                                          
        },
    };
</script>






