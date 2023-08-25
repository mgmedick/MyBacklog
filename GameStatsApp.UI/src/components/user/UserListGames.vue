<template>
    <div class="container games-container p-0">
        <div class="d-flex align-items-center mb-3">
            <div v-if="userlists.find(i => i.defaultListID == 1).id == userlistid" class="me-2">
                <div class="btn btn-secondary p-2" tabindex="-1" role="button">
                    <font-awesome-layers v-if="Object.keys(importingUserAccountIDs).length > 0" class="fa-2xl" @click="onImportClick">
                        <font-awesome-icon icon="fa-solid fa-spinner m-2" spin transform="shrink-5" style="color: #adb5bd; margin-left: 0.25rem; z-index: 9999;"/>
                        <font-awesome-icon icon="fa-solid fa-cloud"/>
                    </font-awesome-layers>
                    <font-awesome-icon v-else icon="fa-solid fa-cloud-arrow-down" size="2xl" @click="onImportClick"/>   
                </div>      
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
                <input type="text" class="form-control" autocomplete="off" v-model="filterText" aria-describedby="spnUserNameErrors" placeholder="Filter games">
            </div>
        </div>
        <div v-if="loading">
            <div class="row g-3">
                <div class="col-lg-2 col-md-3 col-6">
                    <div class="position-relative add-game-container">
                        <div class="position-absolute top-0 bottom-0 start-0 end-0">
                            <div class="d-flex" style="width: 100%; height: 100%;">
                                <font-awesome-icon icon="fa-solid fa-plus" size="2xl" class="mx-auto align-self-center" style="font-size: 50px;"/>                    
                            </div>
                        </div>                  
                        <img src="/dist/images/gamecovers/emptycover.jpg" class="img-fluid rounded" alt="Responsive image">
                    </div>
                </div>
            </div>   
            <div class="center" style="font-size: 25px;">       
                <font-awesome-icon icon="fa-solid fa-spinner" spin size="2xl"/>
            </div>
        </div>
        <div v-else class="row g-3">
            <div class="col-lg-2 col-md-3 col-6">
                <div class="position-relative add-game-container" @click="onSearchGamesClick">
                    <div class="position-absolute top-0 bottom-0 start-0 end-0">
                        <div class="d-flex" style="width: 100%; height: 100%;">
                            <font-awesome-icon icon="fa-solid fa-plus" size="2xl" class="mx-auto align-self-center" style="font-size: 50px;"/>                    
                        </div>
                    </div>                  
                    <img src="/dist/images/gamecovers/emptycover.jpg" class="img-fluid rounded" alt="Responsive image">
                </div>
            </div>
            <div v-for="(game, gameIndex) in games" class="col-lg-2 col-md-3 col-6" key="game.id">
                <div class="position-relative game-image-container rounded d-flex" style="overflow: hidden; background: linear-gradient(45deg,#dbdde3,#fff);" @mouseover="onGameImageMouseOver" @mouseleave="onGameImageMouseLeave" @click="onGameImageClick">
                    <div v-if="game.coverImagePath?.indexOf('nocover.jpg') > -1" class="position-absolute text-center bottom-0 start-0 end-0" style="line-height: 20px; top: 90px;">
                        <small class="position-relative">{{ game.name }}</small>
                    </div>                
                    <div class="delete-icon mt-2 position-absolute start-0 end-0 d-none" style="z-index: 1;">
                        <font-awesome-icon icon="fa-solid fa-circle-xmark" size="xl" class="d-flex ms-auto me-2" style="color: #d9534f; background: radial-gradient(#fff 50%, transparent 50%); cursor: pointer;" @click="onDeleteClick($event, game)"/> 
                    </div>      
                    <img :src="game.coverImagePath" class="img-fluid align-self-center" alt="Responsive image">
                    <div class="gamelist-icons position-absolute start-0 end-0 d-none" style="bottom: 10px; width: 100%; z-index: 1;">
                        <div class="btn-group position-relative px-2" role="group" style="width: 100%;">
                            <button v-for="(userList, userListIndex) in userlists.filter(i => i.defaultListID && i.defaultListID != 1)" :key="userList.id" @click="onUserListClick($event, userList, game)" type="button" class="btn btn-light btn-sm gamelist-item" :class="{ 'active' : game.userListIDs.indexOf(userList.id) > -1 }" :data-val="userList.id">
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
        </div>         
        <div ref="importmodal" class="modal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Import Games</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <import-games ref="importGames" :importgamesvm="importgamesvm" :importinguseraccountids="importingUserAccountIDs" @update:importinguseraccountids="onImportingUserAccountIDsUpdate"></import-games>
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
                        <autocomplete ref="searchAutocomplete" v-model="searchText" @search="onSearch" @selected="onSearchSelected" :options="searchResults" :isasync="true" :isimgresults="true" :loading="searchLoading" :placeholder="'Search games'"/>    
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
            userlists: Array,
            importgamesvm: Object
        },
        data: function () {
            return {
                games: [],
                allgames: [],
                game: {},
                loading: false,
                importModal: {},
                removeModal: {},
                searchModal: {},
                searchText: null,
                searchResults: [],
                searchLoading: false,
                filterText: null,
                orderByDesc: true,             
                orderByID: 0,
                importingUserAccountIDs: JSON.parse(sessionStorage.getItem('importingUserAccountIDs')) ?? {},
                width: document.documentElement.clientWidth,
                height: document.documentElement.clientHeight
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
            window.addEventListener('importingUserAccountIDsUpdate', (event) => {
                that.importingUserAccountIDs = JSON.parse(sessionStorage.getItem('importingUserAccountIDs')) ?? {};
            });

            window.addEventListener('importGamesComplete', (event) => {
                location.href = '/';
            });
            
            that.$refs.searchmodal.addEventListener('hidden.bs.modal', event => {
                that.$refs.searchAutocomplete.clear();
            });  
           
            window.addEventListener('resize', that.onResize);

            that.importModal = new Modal(that.$refs.importmodal);
            that.removeModal = new Modal(that.$refs.removemodal);
            that.searchModal = new Modal(that.$refs.searchmodal);

            if (that.importgamesvm.authSuccess != null) {
                if (that.importgamesvm.authSuccess) {
                    var userAccount = that.importgamesvm.userAccounts.find(i => i.accountTypeID == that.importgamesvm.authAccountTypeID);
                    that.$refs.importGames.ImportGames(userAccount);
                    that.importModal.show();
                } else {            
                    that.errorToast("Error authorizing account");       
                }
            }            
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
            onImportClick(e) {
                this.importModal.show();
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
                            that.searchLoading = false;

                            return res;
                        })
                        .catch(err => { console.error(err); return Promise.reject(err); });
            },              
            onSearchSelected: function (result) {
                var that = this;
                var userlist = this.userlists.find(item => item.id == that.userlistid);

                that.addNewGameToUserList(userlist, result.value).then(i => { that.searchModal.hide() });
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
            onImportingUserAccountIDsUpdate: function (result) {
                this.importingUserAccountIDs = result;
                sessionStorage.setItem('importingUserAccountIDs', JSON.stringify(this.importingUserAccountIDs));
            },       
            sortGames() {
                switch (this.orderByID)
                {
                    case 0:
                        if (this.orderByDesc) {
                            this.games = this.games.sort((a, b) => { return b.userListGameID - a.userListGameID });
                            this.allgames = this.allgames.sort((a, b) => { return b.userListGameID - a.userListGameID });
                        } else {
                            this.games = this.games.sort((a, b) => { return a.userListGameID - b.userListGameID });
                            this.allgames = this.allgames.sort((a, b) => { return a.userListGameID - b.userListGameID });
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
            addNewGameToUserList(userList, gameID) {
                var that = this;  

                return axios.post('/User/AddNewGameToUserList', null,{ params: { userListID: userList.id, gameID: gameID } })
                    .then((res) => {
                        if (res.data.success) {
                            var game = res.data.game;

                            if (that.games.filter(i => i.id == game.id).length == 0) {
                                that.games.push(game);
                                that.allgames.push({...game});
                                that.sortGames();
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
                el.closest('.gamelist-btn-group button')?.classList.remove('active');

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
            },
            resizeColumns() {
                var defaultheight = document.querySelector('.add-game-container').clientHeight;
                if (defaultheight > 0) {
                    document.querySelectorAll('.game-image-container').forEach(item => {
                        item.style.height = defaultheight + 'px';
                    });
                }
            }                                                                                          
        },
    };
</script>






