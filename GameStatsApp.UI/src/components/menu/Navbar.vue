<template>
    <nav class="navbar navbar-expand-lg navbar-light bg-light">
        <div class="container-fluid">
            <a class="navbar-brand" href="/" draggable="false">
                <font-awesome-icon icon="fa-solid fa-cube" class="me-2"/>
                <span class="me-2">mybacklog.io</span>
                <span v-if="isdemo" class="text-warning">demo</span>
            </a>
            <button class="navbar-toggler" type="button" @click="toggleNavbar = !toggleNavbar" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div id="navbarNav" class="navbar-collapse" :style="[ toggleNavbar ? null : { display:'none' } ]">
                <ul class="navbar-nav me-auto mb-2 mb-lg-0">
                    <li class="nav-item">
                        <a class="nav-link" href="/Menu/About">About</a>
                    </li>                    
                </ul>
                <div class="d-flex">
                    <!-- <autocomplete v-model="searchText" class="me-2" @change="onChange" @search="onSearch" @selected="onSearchSelected" :options="searchResults" :isasync="true" :loading="searchLoading" :placeholder="'Search users'"/> -->
                    <div v-if="isauth" class="ms-auto">
                        <div class="btn-group">
                            <button class="btn dropdown-toggle btn-secondary" type="button" data-bs-toggle="dropdown" data-bs-display="static" aria-expanded="false">
                                <font-awesome-icon icon="fa-solid fa-user" size="xl" />
                            </button>
                            <ul class="dropdown-menu dropdown-menu-end" aria-labelledby="dropdownMenuButton">
                                <li>
                                    <a href="/UserSettings" class="dropdown-item"><font-awesome-icon icon="fa-solid fa-gear" /><span class="ps-2">Settings</span></a>
                                </li>
                                <li>
                                    <a href="/Logout" class="dropdown-item"><font-awesome-icon icon="fa-solid fa-right-from-bracket" /><span class="ps-2">Log out</span></a>
                                </li>
                            </ul>
                        </div>
                    </div>
                    <ul v-else class="navbar-nav">
                        <li class="nav-item">
                            <a class="nav-link" href="/Login">Log In</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="/SignUp">Sign Up</a>
                        </li>          
                        <li class="nav-item">
                            <a class="nav-link" href="#/" @click="onDemoClick">Demo</a>
                        </li>                                      
                    </ul>
                </div>
            </div>
        </div> 
    </nav>       
</template>
<script>
    import axios from 'axios'
    import { successToast, errorToast, getCookie, listenCookieChange } from '../../js/common';
    
    export default {
        name: "Navbar",
        props: {
            isauth: Boolean,
            username: String,
            userid: String,
            isdemo: Boolean
        },
        data: function () {
            return {
                searchText: null,
                searchResults: [],
                searchLoading: false,
                showLoginModal: false,
                showResetModal: false,
                showSignUpModal: false,
                showDropdown: false,
                toggleNavbar: false,
                state: false
            }
        },       
        computed: {
        },
        created: function () {
        },         
        mounted() {
            var that = this;
            var importingUserAccountIDs = JSON.parse(sessionStorage.getItem('importingUserAccountIDs')) ?? {};
            
            if (Object.keys(importingUserAccountIDs).length > 0) {
                var intervalID = setInterval(function() {
                    axios.get('/User/GetCompletedImportGames')
                        .then(res => {
                            var results = res.data;

                            if (Object.keys(results).length > 0) {
                                Object.keys(importingUserAccountIDs).forEach(userAccountID => {
                                    if (results.hasOwnProperty(userAccountID)) {
                                        if (results[userAccountID].item1) {
                                            successToast("Imported <strong>" + results[userAccountID].item2 + "</strong> new games into <strong>" + importingUserAccountIDs[userAccountID].userListName + "</strong>");
                                        } else {
                                            errorToast("Error importing into <strong>" + importingUserAccountIDs[userAccountID].userListName  + "</strong>");
                                        }

                                        delete importingUserAccountIDs[userAccountID];
                                    }
                                });
                                
                                sessionStorage.setItem('importingUserAccountIDs', JSON.stringify(importingUserAccountIDs));
                                window.dispatchEvent(new CustomEvent('importingUserAccountIDsUpdate'));
                            }
                        })
                        .catch(err => { console.error(err); return Promise.reject(err); });
                }, 10000);
            }
        },        
        methods: {
            onDemoClick() {
                axios.post('/Home/LoginDemo')
                    .then((res) => {
                        if (res.data.success) {
                            location.href = '/';
                        } else {
                            res.data.errorMessages.forEach(errorMsg => {
                                errorToast(errorMsg);                           
                            }); 
                        }
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },
            onInput: function(e){
                this.searchText = e;
            },
            onChange: function() {
                var a = this.searchText;
            },            
            onSearch: function() {
                var that = this;
                this.searchLoading = true;
                               
                axios.get('/Menu/Search', { params: { term: this.searchText } })
                        .then(res => {
                            that.searchResults = res.data.reduce((flat, groupheader) => {
                                return flat
                                    .concat({
                                        label: groupheader.label,
                                        value: groupheader.subItems.map(method => method.value),
                                        isGroupHeader: true,
                                        disabled: true
                                    })
                                    .concat(groupheader.subItems.map(method => ({ label: method.label, value: method.value, category: groupheader.label })))
                            }, []);

                            if(that.searchResults.length == 0)
                            {
                                var noResult = { value: "", label: "No results found", category: null, disabled: true };
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

                if (result.category == 'Users') {
                    controller = "User";
                    action = "UserDetails"
                }
                
                location.href = encodeURI('/' + controller + "/" + action + "/" + result.value);
            },           
            toggleDropdown(e) {
                this.state = !this.state
            }
        }
    };
</script>






