<template>
    <div class="games-container">
        <div class="container-fluid m-0 p-0">
            <div class="d-flex align-items-end mb-3">
                <div class="me-2">
                    <div class="btn btn-secondary p-2" tabindex="-1" role="button" @click="onImportClick">
                        <font-awesome-layers v-if="isImporting" class="fa-xl" style="width: 26px; z-index: 0;">
                            <font-awesome-icon icon="fa-solid fa-spinner" spin transform="shrink-4" style="color: #adb5bd;"/>
                            <font-awesome-icon icon="fa-solid fa-cloud" style="z-index: -1;"/>
                        </font-awesome-layers>
                        <font-awesome-icon v-else icon="fa-solid fa-cloud-arrow-down" size="xl"/>   
                    </div>      
                </div>              
                <div class="d-flex ms-auto">   
                    <div class="btn-group me-2" role="group">
                        <button type="button" class="btn btn-secondary p-2" @click="onOrderByDescClick"><font-awesome-icon v-if="orderByDesc" icon="fa-solid fa-arrow-up-wide-short" size="xl"/><font-awesome-icon v-else icon="fa-solid fa-arrow-down-wide-short" size="xl"/></button>
                        <button type="button" class="btn btn-secondary dropdown-toggle dropdown-toggle-split p-2" data-bs-toggle="dropdown" aria-expanded="false">
                            <span class="visually-hidden">Toggle Dropdown</span>
                        </button>                    
                        <ul class="dropdown-menu">
                            <li><a href="#/" @click="onOrderByOptionClick($event, 0)" class="dropdown-item" :class="{ 'active' : orderByID == 0 }">Date Added</a></li>
                            <li><a href="#/" @click="onOrderByOptionClick($event, 1)" class="dropdown-item" :class="{ 'active' : orderByID == 1 }">Name</a></li>
                        </ul>
                    </div>
                    <div class="input-group">
                        <button type="button" class="btn btn-secondary p-2" @click="onShowFilterTextClick"><font-awesome-icon icon="fa-solid fa-filter" size="xl"/></button> 
                        <input v-if="showFilterText" v-model="filterText" type="search" class="form-control" placeholder="Filter games">
                    </div>
                </div>
            </div>            
            <div v-if="loading">
                <div class="row g-3">
                    <div class="col-xl-auto col-md-3 col-6">
                        <div class="position-relative add-game-container" role="button">
                            <div class="position-absolute top-0 bottom-0 start-0 end-0">
                                <div class="d-flex" style="width: 100%; height: 100%;">
                                    <font-awesome-icon icon="fa-solid fa-plus" size="2xl" class="mx-auto align-self-center" style="font-size: 50px;"/>                    
                                </div>
                            </div>                  
                            <img :src="emptycoverimagepath" class="img-fluid rounded" alt="Responsive image">
                        </div>
                    </div>
                </div>   
                <div class="center" style="font-size: 25px;">       
                    <font-awesome-icon icon="fa-solid fa-spinner" spin size="2xl"/>
                </div>
            </div>
            <div v-else>
                <div v-if="games.length == 0 && userlistid == 0" class="center text-center">
                    <div class="text-muted lead">
                        <span>Add <font-awesome-icon icon="fa-solid fa-plus"/> or import <font-awesome-icon icon="fa-solid fa-cloud-arrow-down"/> games to your lists to get started</span>
                    </div>
                </div>   
                <div v-else class="row g-3">        
                    <div class="col-xl-auto col-md-3 col-6">
                        <div class="position-relative add-game-container" role="button" @click="onSearchGamesClick">
                            <div class="position-absolute top-0 bottom-0 start-0 end-0">
                                <div class="d-flex" style="width: 100%; height: 100%;">
                                    <font-awesome-icon icon="fa-solid fa-plus" size="2xl" class="mx-auto align-self-center" style="font-size: 50px;"/>                    
                                </div>
                            </div>                  
                            <img :src="emptycoverimagepath" class="img-fluid rounded" alt="Responsive image">
                        </div>
                    </div>
                    <div v-for="(game, gameIndex) in games" class="col-xl-auto col-md-3 col-6" :key="game.id">
                        <div class="position-relative game-image-container rounded d-flex" style="overflow: hidden; background: linear-gradient(45deg,#dbdde3,#fff);" @mouseover="onGameImageMouseOver" @mouseleave="onGameImageMouseLeave" @click="onGameImageClick">
                            <div v-if="game.coverImagePath?.indexOf('nocover.png') > -1" class="position-absolute text-center bottom-0 start-0 end-0 px-1" style="line-height: 20px; top: 10px; z-index: 1;">
                                <small class="position-relative text-dark">{{ game.name }}</small>
                            </div>                
                            <div class="delete-icon mt-2 position-absolute start-0 end-0 d-none" style="z-index: 1;">
                                <div class="d-flex">
                                    <font-awesome-icon icon="fa-solid fa-circle-xmark" size="xl" class="ms-auto me-2" style="color: #d9534f; background: radial-gradient(#fff 50%, transparent 50%); cursor: pointer;" @click="onDeleteClick($event, game)"/> 
                                </div>
                            </div>      
                            <img :src="game.coverImagePath" class="img-fluid align-self-center" :style="[ game.coverImagePath?.indexOf('nocover.png') > -1 ? { opacity:'0.5' } : null ]" alt="Responsive image">
                            <div class="gamelist-icons position-absolute start-0 end-0 d-none" style="bottom: 10px; width: 100%; z-index: 1;">
                                <div class="btn-group btn-group-sm position-relative px-2" role="group" style="width: 100%;">
                                    <button v-for="(userList, userListIndex) in userlists.filter(i => i.defaultListID)" :key="userList.id" @click="onUserListClick($event, userList, game)" type="button" class="btn btn-light btn-sm gamelist-item" :class="{ 'active' : game.userListIDs.indexOf(userList.id) > -1 }" :data-val="userList.id">
                                        <font-awesome-icon :icon="getIconClass(userList.defaultListID)" size="lg"/>
                                    </button>
                                    <template v-if="userlists.filter(i => !i.defaultListID).length > 0">
                                        <button type="button" class="btn btn-light dropdown-toggle" :class="{ 'active' : userlists.filter(i => !i.defaultListID && game.userListIDs.indexOf(i.id) > -1).length > 0 }" data-bs-toggle="dropdown" aria-expanded="false">
                                            <font-awesome-icon v-if="userlists.filter(i => !i.defaultListID && game.userListIDs.indexOf(i.id) > -1).length == 0" icon="fa-solid fa-ellipsis" size="lg"/>
                                            <span v-else class="fw-bold">{{ '(+' + userlists.filter(i => !i.defaultListID && game.userListIDs.indexOf(i.id) > -1).length + ')' }}</span>
                                        </button>
                                        <ul class="dropdown-menu">
                                            <li v-for="(userList, userListIndex) in userlists.filter(i => !i.defaultListID)" :key="userList.id"><a @click="onUserListClick($event, userList, game)" href="#/" class="dropdown-item gamelist-item" :class="{ 'active' : game.userListIDs.indexOf(userList.id) > -1 }">{{ userList.name }}</a></li>
                                        </ul>
                                    </template>
                                </div>
                            </div>
                        </div> 
                    </div>
                </div>                      
            </div>             
        </div>       
        <div ref="importmodal" class="modal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Import Games</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <import-games ref="importgames" @isimportingupdate="onIsImportingUpdate"></import-games>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    </div>                      
                </div>
            </div>
        </div>         
        <div ref="removemodal" class="modal" tabindex="-1">
            <div class="modal-dialog">
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
                        <autocomplete ref="searchautocomplete" v-model="searchText" @search="onSearch" @selected="onSearchSelected" :options="searchResults" :isasync="true" :isimgresults="true" :loading="searchLoading" :placeholder="'Search games'"/>    
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
            userlistid: Number,
            userlists: Array,
            emptycoverimagepath: String,
            showimport: Boolean
        },
        data: function () {
            return {
                games: [],
                allgames: [],
                game: {},
                loading: false,
                searchText: null,
                searchResults: [],
                searchLoading: false,
                filterText: null,
                showFilterText: false,
                orderByDesc: sessionStorage.getItem('orderByDesc') ? sessionStorage.getItem('orderByDesc') == 'true' : false,             
                orderByID: sessionStorage.getItem('orderByID') ?? 0,
                isImporting: Object.keys(JSON.parse(sessionStorage.getItem('importingUserAccountIDs')) ?? {}).length > 0,
                width: document.documentElement.clientWidth,
                height: document.documentElement.clientHeight
            };
        },  
        computed: {     
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
            window.addEventListener('importingUserAccountIDsUpdate', (event) => {
                that.isImporting = Object.keys(JSON.parse(sessionStorage.getItem('importingUserAccountIDs')) ?? {}).length > 0;
                
                if (!that.isImporting) {
                    that.loadData();
                }
            });

            that.$refs.importmodal.addEventListener('show.bs.modal', event => {
                that.$refs.importgames.init();
            }); 

            that.$refs.searchmodal.addEventListener('hidden.bs.modal', event => {
                that.$refs.searchautocomplete.clear();
            });            

            window.addEventListener('resize', that.onResize);
            
            if (that.showimport) {
                new Modal(that.$refs.importmodal).show();
            }
        },  
        updated: function() {
            var that = this;
            
            that.$nextTick(function() {
                if (that.userlistid == 0) {
                    document.querySelector('.add-game-container')?.parentElement.classList.add('d-none');
                }
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

                        that.$nextTick(function() {
                            that.resizeColumns();
                        });

                        return res;
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },  
            getIconClass: function (id) {
                var iconClass = '';

                switch (id) {
                    case 1:
                        iconClass = 'fa-solid fa-inbox';
                        break;
                    case 2:
                        iconClass = 'fa-solid fa-play';
                        break;
                    case 3:
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
            onImportClick(e) {
                new Modal(this.$refs.importmodal).show();
            },
            onDeleteClick(e, game) {
                var that = this;
                var el = e.target;
                
                if (!el.closest('.delete-icon').classList.contains('d-none')){
                    that.game = game;
                    new Modal(that.$refs.removemodal).show();
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
                new Modal(this.$refs.searchmodal).show();
            },         
            onSearch: function() {
                var that = this;
                this.searchLoading = true;
                               
                axios.get('/Game/SearchGames', { params: { term: this.searchText } })
                        .then(res => {
                            that.searchResults = res.data;
                            that.searchLoading = false;

                            return res;
                        })
                        .catch(err => { console.error(err); return Promise.reject(err); });
            },              
            onSearchSelected: function (result) {
                var that = this;
                that.addNewGameToUserList(that.userlistid, result.value);
            },  
            onFilterInput: function(e) {
                var val = e.target.value;
                if (val) {
                    this.filterResults(val);
                }
            },
            onOrderByDescClick(e){
                this.orderByDesc = !this.orderByDesc;
                sessionStorage.setItem('orderByDesc', this.orderByDesc.toString());
                this.sortGames();                
            },            
            onOrderByOptionClick(e, val){
                this.orderByID = val;
                sessionStorage.setItem('orderByID', this.orderByID.toString());
                this.sortGames();                
            },
            onShowFilterTextClick(e) {
                this.showFilterText = !this.showFilterText;
                if(!this.showFilterText) {
                    this.filterText = '';
                }
            },
            onResize: function() {
                var that = this;
                if (that.width != document.documentElement.clientWidth || that.height != document.documentElement.clientHeight) {     
                    that.width = document.documentElement.clientWidth;
                    that.height = document.documentElement.clientHeight;   

                    that.$nextTick(function() {
                        that.resizeColumns();
                    });
                }
            }, 
            onIsImportingUpdate(e) {
                this.isImporting = e;

                if (!this.isImporting) {
                    this.loadData();
                }
            },     
            sortGames() {
                switch (this.orderByID.toString())
                {
                    case '0':
                        if (this.orderByDesc) {
                            this.games = this.games.sort((a, b) => { return b.userListGameID - a.userListGameID });
                            this.allgames = this.allgames.sort((a, b) => { return b.userListGameID - a.userListGameID });
                        } else {
                            this.games = this.games.sort((a, b) => { return a.userListGameID - b.userListGameID });
                            this.allgames = this.allgames.sort((a, b) => { return a.userListGameID - b.userListGameID });
                        }
                        break;                    
                    case '1':
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
            addNewGameToUserList(userListID, gameID) {
                var that = this;

                return axios.post('/User/addNewGameToUserList', null,{ headers: { 'RequestVerificationToken': that.getCsrfToken() },
                                                                       params: { userListID: userListID, gameID: gameID } })
                    .then((res) => {
                        if (res.data.success) {                            
                            var result = res.data.result;

                            if (that.games.filter(i => i.id == result.id).length == 0) {
                                that.games.push(result);
                                that.allgames.push({...result});
                                that.sortGames();
                            }

                            Modal.getInstance(that.$refs.searchmodal).hide();

                            var userList = that.userlists.find(i => i.id == userListID);
                            successToast("Added <strong>" + result.name + "</strong> to <strong>" + userList.name + "</strong>");
                        } else {       
                            res.data.errorMessages.forEach(errorMsg => {
                                errorToast(errorMsg);                           
                            });                              
                        }                        
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },                       
            addGameToUserList(userList, game, el) {
                var that = this;

                el.closest('.gamelist-item').classList.add('active');
                el.closest('.dropdown-menu')?.previousSibling.classList.add('active');

                return axios.post('/User/AddGameToUserList', null,{ headers: { 'RequestVerificationToken': that.getCsrfToken() },
                                                                    params: { userListID: userList.id, gameID: game.id } })
                    .then((res) => {
                        if (res.data.success) {                            
                            if (game.userListIDs.indexOf(userList.id) == -1) {
                                game.userListIDs.push(userList.id);
                            }

                            successToast("Added <strong>" + game.name + "</strong> to <strong>" + userList.name + "</strong>");
                        } else {       
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
                el.closest('.dropdown-menu')?.previousSibling.classList.remove('active');

                return axios.post('/User/RemoveGameFromUserList', null,{ headers: { 'RequestVerificationToken': that.getCsrfToken() },
                                                                         params: { userListID: userList.id, gameID: game.id } })
                    .then((res) => {
                        if (res.data.success) {                            
                            game.userListIDs = game.userListIDs.filter(i => i != userList.id);

                            if (that.userlistid == userList.id) {
                                that.games = that.games.filter(i => i.id != game.id);            
                                that.allgames = that.allgames.filter(i => i.id != game.id);  
                            }

                            successToast("Removed <strong>" + game.name + "</strong> from <strong>" + userList.name + "</strong>");
                        } else {
                            res.data.errorMessages.forEach(errorMsg => {
                                errorToast(errorMsg);                           
                            });                           
                        }                        
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },
            removeGameFromAllUserLists(game) {
                var that = this;

                return axios.post('/User/RemoveGameFromAllUserLists', null,{ headers: { 'RequestVerificationToken': that.getCsrfToken() },
                                                                             params: { gameID: game.id } })
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
                        Modal.getInstance(that.$refs.removemodal).hide();                                           
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });                
            },
            resizeColumns() {
                var that = this;

                var defaultheight = document.querySelector('.add-game-container .img-fluid')?.clientHeight;
                if (defaultheight > 0) {
                    document.querySelectorAll('.game-image-container').forEach(item => {
                        item.style.height = defaultheight + 'px';
                    });
                }

                var defaultwidth = document.querySelector('.add-game-container .img-fluid')?.clientWidth;
                if (defaultwidth > 0) {
                    document.querySelectorAll('.game-image-container').forEach(item => {
                        item.style.width = defaultwidth + 'px';
                    });
                }
            }                                                                                          
        },
    };
</script>






